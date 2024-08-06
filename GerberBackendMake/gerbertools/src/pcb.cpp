/**
 * MIT License
 *
 * Copyright (c) 2021 Jeroen van Straten
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

 /** \file
  * Contains the CircuitBoard class, representing a complete circuit board as
  * reconstructed from Gerber files and an NC drill file.
  */

#define _USE_MATH_DEFINES
#include <cmath>
#include "gerber.hpp"
#include "pcb.hpp"
#include "path.hpp"
#include "../../src/BoardEnumTypes.h"
#include <iostream>
#include <stdexcept>
#include <chrono>
#include <string>
#include <vector>
#include <filesystem>

namespace gerbertools {
	namespace pcb {

		/**
		 * Creates a new color scheme.
		 */
		ColorScheme::ColorScheme(
			const color::Color& soldermask,
			const color::Color& silkscreen,
			const color::Color& finish,
			const color::Color& substrate,
			const color::Color& copper
		) :
			soldermask(soldermask),
			silkscreen(silkscreen),
			finish(finish),
			substrate(substrate),
			copper(copper)
		{}

		/**
		 * Constructs a layer.
		 */
		Layer::Layer(const std::string& name, double thickness) : name(name), thickness(thickness) {
		}

		/**
		 * Returns a name for the layer.
		 */
		std::string Layer::get_name() const {
			return name;
		}

		/**
		 * Returns the thickness of this layer.
		 */
		double Layer::get_thickness() const {
			return thickness;
		}



		/**
		 * Constructs a substrate layer.
		 */
		SubstrateLayer::SubstrateLayer(
			const std::string& name,
			const coord::Paths& shape,
			const coord::Paths& dielectric,
			const coord::Paths& plating,
			double thickness
		) : Layer(name, thickness), shape(shape), dielectric(dielectric), plating(plating) {
		}

		/**
		 * Returns the surface finish mask for this layer.
		 */
		coord::Paths SubstrateLayer::get_mask() const {
			return dielectric;
		}

		/**
		 * Renders the layer to an SVG layer.
		 */
		svg::Layer SubstrateLayer::to_svg(const ColorScheme& colors, bool flipped, const std::string& id_prefix) const {
			auto layer = svg::Layer(id_prefix + get_name());
			layer.add(dielectric, colors.substrate);
			layer.add(plating, colors.finish);
			return layer;
		}

		/**
		 * Renders the layer to an OBJ file.
		 */
		void SubstrateLayer::to_obj(obj::ObjFile& obj, size_t layer_index, double z, const std::string& id_prefix) const {
			obj.add_object(
				"layer" + std::to_string(layer_index) + "_" + get_name(),
				"substrate"
			).add_sheet(
				dielectric,
				z,
				z + get_thickness()
			);
		}

		/**
		 * Constructs a copper layer.
		 */
		CopperLayer::CopperLayer(
			const std::string& name,
			const coord::Paths& board_shape,
			const coord::Paths& board_shape_excl_pth,
			const coord::Paths& copper_layer,
			double thickness
		) : Layer(name, thickness) {
			layer = copper_layer;
			copper = path::intersect(board_shape, copper_layer);
			copper_excl_pth = path::intersect(board_shape_excl_pth, copper_layer);
		}

		/**
		 * Returns the surface finish mask for this layer.
		 */
		coord::Paths CopperLayer::get_mask() const {
			return copper;
		}

		/**
		 * Returns the copper for this layer.
		 */
		const coord::Paths& CopperLayer::get_copper() const {
			return copper;
		}

		/**
		 * Returns the copper for this layer, with only cutouts for board shape and
		 * non-plated holes, not for plated holes. This is needed for annular ring DRC.
		 */
		const coord::Paths& CopperLayer::get_copper_excl_pth() const {
			return copper_excl_pth;
		}

		/**
		 * Returns the original layer, without board outline intersection.
		 */
		const coord::Paths& CopperLayer::get_layer() const {
			return layer;
		}

		/**
		 * Renders the layer to an SVG layer.
		 */
		svg::Layer CopperLayer::to_svg(const ColorScheme& colors, bool flipped, const std::string& id_prefix) const {
			auto layer = svg::Layer(id_prefix + get_name());
			layer.add(copper, colors.copper);
			return layer;
		}

		/**
		 * Renders the layer to an OBJ file.
		 */
		void CopperLayer::to_obj(obj::ObjFile& obj, size_t layer_index, double z, const std::string& id_prefix) const {
			// No-operation. Copper is added via the physical netlist, such that each
			// bit of connected copper gets its own object.
		}

		/**
		 * Constructs a solder mask.
		 */
		MaskLayer::MaskLayer(
			const std::string& name,
			const coord::Paths& board_outline,
			const coord::Paths& mask_layer,
			const coord::Paths& silk_layer,
			bool bottom
		) : Layer(name, 0.01), bottom(bottom) {
			mask = path::subtract(board_outline, mask_layer);
			silk = path::intersect(mask, silk_layer);
		}

		/**
		 * Returns the surface finish mask for this layer.
		 */
		coord::Paths MaskLayer::get_mask() const {
			return mask;
		}

		/**
		 * Renders the layer to an SVG layer.
		 */
		svg::Layer MaskLayer::to_svg(const ColorScheme& colors, bool flipped, const std::string& id_prefix) const {
			auto layer = svg::Layer(id_prefix + get_name());
			if (bottom == flipped) {
				layer.add(mask, colors.soldermask);
				layer.add(silk, colors.silkscreen);
			}
			else {
				layer.add(silk, colors.silkscreen);
				layer.add(mask, colors.soldermask);
			}
			return layer;
		}

		/**
		 * Renders the layer to an OBJ file.
		 */
		void MaskLayer::to_obj(obj::ObjFile& obj, size_t layer_index, double z, const std::string& id_prefix) const {
			double mask_z1, mask_z2, silk_z;
			std::string mask_name, silk_name;
			if (bottom) {
				mask_name = "_GBS";
				silk_name = "_GBO";
				silk_z = z;
			}
			else {
				mask_name = "_GTS";
				silk_name = "_GTO";
				silk_z = z + get_thickness();
			}

			// The soldermask is probably transparent, which makes Z-fighting a
			// potential issue. Just work around it by making the soldermask slightly
			// thinner than it should be. The same applies for the silkscreen layer,
			// which is modelled without thickness.
			mask_z1 = z + get_thickness() * 0.01;
			mask_z2 = z + get_thickness() * 0.99;

			obj.add_object(
				"layer" + std::to_string(layer_index) + mask_name,
				"soldermask"
			).add_sheet(
				mask,
				mask_z1,
				mask_z2
			);
			obj.add_object(
				"layer" + std::to_string(layer_index) + silk_name,
				"silkscreen"
			).add_surface(
				silk,
				silk_z
			);
		}

		/**
		 * Returns an open file input stream for the given filename.
		 */
		std::istringstream CircuitBoard::read_file(const std::string& buffer) {
			std::string str(buffer.begin(), buffer.end());
			return std::istringstream(str);
		}

		/**
		 * Reads a Gerber file.
		 */
		//coord::Paths CircuitBoard::read_gerber(const std::vector<char>& fname, bool outline) {
		//	if (fname.empty()) {
		//		return {};
		//	}

		//	std::string stringName;

		//	auto lol = fname.at(0);

		//	auto f = read_file(fname);
		//	auto g = gerber::Gerber(f);
		//	auto paths = outline ? g.get_outline_paths() : g.get_paths();
		//	return paths;
		//}

		coord::Paths CircuitBoard::read_gerber(const std::string& fname, bool outline) {
			if (fname.empty()) {
				return {};
			}
			auto f = std::istringstream(fname);
			auto g = gerber::Gerber(f);
			auto paths = outline ? g.get_outline_paths() : g.get_paths();
			return paths;
		}

		/**
		 * Reads an NC drill file.
		 */
		void CircuitBoard::read_drill(const std::string& fname, bool plated, coord::Paths& pth, coord::Paths& npth) {
			if (fname.empty()) {
				return;
			}
			//std::cout << "reading drill file " << fname << "..." << std::endl;
			auto f = read_file(fname);
			auto d = ncdrill::NCDrill(f, plated);
			auto l = d.get_paths(true, false);
			if (pth.empty()) {
				pth = l;
			}
			else {
				pth.insert(pth.end(), l.begin(), l.end());
				ClipperLib::SimplifyPolygons(pth);
			}
			l = d.get_paths(false, true);
			if (npth.empty()) {
				npth = l;
			}
			else {
				npth.insert(npth.end(), l.begin(), l.end());
				ClipperLib::SimplifyPolygons(npth);
			}
			auto new_vias = d.get_vias();
			vias.insert(vias.end(), new_vias.begin(), new_vias.end());
		}


		/**
		 * Constructs a circuit board. The following files are to be specified:
		 *  - basename: prefix for all filenames.
		 *  - outline: the board outline Gerber file (GKO, GM1, etc). May also
		 *    include milling information.
		 *  - drill: the NC drill file (TXT).
		 *  - drill_nonplated: if specified, non-plated holes will be added from
		 *    this auxiliary NC drill file.
		 *  - mill: if specified, adds another Gerber-based milling layer.
		 *    Interpreted in the same way as outline.
		 *  - plating_thickness: thickness of the hole plating in millimeters.
		 */
		//CircuitBoard::CircuitBoard(
		//	const std::string& basename,
		//	const std::string& outline,
		//	const std::string& drill,
		//	const std::string& drill_nonplated,
		//	const std::string& mill,
		//	double plating_thickness
		//) : basename(basename), num_substrate_layers(0), plating_thickness(coord::Format::from_mm(plating_thickness)) {

		//	// Load board outline.
		//	board_outline = read_gerber(outline, true);
		//	coord::Paths pth, npth;
		//	//read_drill(mill, true, pth, npth);
		//	path::append(board_outline, read_gerber(mill, true));

		//	// Load drill files.

		//	read_drill(drill, true, pth, npth);
		//	if (!drill_nonplated.empty()) {
		//		read_drill(drill_nonplated, false, pth, npth);
		//	}

		//	// Make board shape.
		//	auto holes = path::add(pth, npth);
		//	board_shape = path::subtract(board_outline, holes);
		//	board_shape_excl_pth = path::subtract(board_outline, npth);

		//	// Build plating.
		//	coord::Paths pth_drill = path::offset(pth, this->plating_thickness, true);

		//	// Make substrate shape.
		//	substrate_dielectric = path::subtract(board_outline, path::add(pth_drill, npth));
		//	substrate_plating = path::subtract(pth_drill, pth);

		//}

		CircuitBoard::CircuitBoard(
			const std::string& basename,
			std::string& outline,
			std::vector<std::string>& drill,
			std::string& drill_nonplated,
			std::string& mill,
			double plating_thickness
		) : basename(basename), num_substrate_layers(0), plating_thickness(coord::Format::from_mm(0.5 * COPPER_OZ)){

			std::string outline_str(outline.begin(), outline.end());
			board_outline = read_gerber(outline_str, true);
			coord::Paths pth, npth;
			path::append(board_outline, read_gerber("", true));

			for each (auto var in drill)
			{
				read_drill(var, true, pth, npth);
				if (drill_nonplated.empty()) {
					read_drill(var, false, pth, npth);
				}
			}
			auto holes = path::add(pth, npth);
			board_shape = path::subtract(board_outline, holes);
			board_shape_excl_pth = path::subtract(board_outline, npth);

			coord::Paths pth_drill = path::offset(pth, this->plating_thickness, true);

			substrate_dielectric = path::subtract(board_outline, path::add(pth_drill, npth));
			substrate_plating = path::subtract(pth_drill, pth);

		}

		/**
		 * Adds a mask layer to the board. Layers are added bottom-up.
		 */
		void CircuitBoard::add_mask_layer(std::string& mask, const std::string& silk) {
			layers.push_back(std::make_shared<MaskLayer>(
				"mask" + mask, board_outline, read_gerber(mask), read_gerber(silk), layers.empty()
			));
		}

		/**
		 * Adds a copper layer to the board. Layers are added bottom-up.
		 */
		void CircuitBoard::add_copper_layer(std::string& gerber, double thickness) {
			layers.push_back(std::make_shared<CopperLayer>(
				"copper" + gerber, board_shape, board_shape_excl_pth, read_gerber(gerber), thickness
			));
		}

		/**
		 * Adds a substrate layer. Layers are added bottom-up.
		 */
		void CircuitBoard::add_substrate_layer(double thickness) {
			layers.push_back(std::make_shared<SubstrateLayer>(
				"substrate" + std::to_string(++num_substrate_layers), board_shape, substrate_dielectric, substrate_plating, thickness
			));
		}

		/**
		 * Derives the surface finish layer for all exposed copper.
		 */
		void CircuitBoard::add_surface_finish() {
			coord::Paths mask;
			for (auto it = layers.begin(); it != layers.end(); ++it) {
				auto copper = std::dynamic_pointer_cast<CopperLayer>(*it);
				if (copper) {
					bottom_finish = path::subtract(copper->get_copper(), mask);
					break;
				}
				mask = path::add(mask, (*it)->get_mask());
			}
			mask.clear();
			for (auto it = layers.rbegin(); it != layers.rend(); ++it) {
				auto copper = std::dynamic_pointer_cast<CopperLayer>(*it);
				if (copper) {
					top_finish = path::subtract(copper->get_copper(), mask);
					break;
				}
				mask = path::add(mask, (*it)->get_mask());
			}
		}

		/**
		 * Returns a netlist builder initialized with the vias and copper regions of
		 * this PCB.
		 */
		netlist::NetlistBuilder CircuitBoard::get_netlist_builder() const {
			netlist::NetlistBuilder nb;
			for (const auto& layer : layers) {
				auto copper = std::dynamic_pointer_cast<CopperLayer>(layer);
				if (copper) {
					nb.layer(copper->get_copper_excl_pth());
				}
			}
			for (const auto& via : vias) {
				nb.via(via.get_path(), via.get_finished_hole_size(), plating_thickness);
			}
			return nb;
		}

		/**
		 * Returns the physical netlist for this PCB.
		 */
		netlist::PhysicalNetlist CircuitBoard::get_physical_netlist() const {
			netlist::PhysicalNetlist pn;
			size_t layer_index = 0;
			for (const auto& layer : layers) {
				auto copper = std::dynamic_pointer_cast<CopperLayer>(layer);
				if (copper) {
					pn.register_paths(copper->get_copper_excl_pth(), layer_index++);
				}
			}
			for (const auto& via : vias) {
				pn.register_via(
					std::make_shared<netlist::Via>(
						via.get_path(),
						via.get_finished_hole_size(),
						plating_thickness
					),
					layer_index
				);
			}
			return pn;
		}

		/**
		 * Returns the axis-aligned boundary coordinates of the PCB.
		 */
		coord::CRect CircuitBoard::get_bounds() const {
			coord::CRect bounds;
			bounds.left = bounds.bottom = INT64_MAX;
			bounds.right = bounds.top = INT64_MIN;
			for (const auto& path : board_outline) {
				for (const auto& point : path) {
					bounds.left = std::min(bounds.left, point.X);
					bounds.right = std::max(bounds.right, point.X);
					bounds.bottom = std::min(bounds.bottom, point.Y);
					bounds.top = std::max(bounds.top, point.Y);
				}
			}
			return bounds;
		}

		/**
		 * Renders the circuit board to SVG, returning only the body of it, allowing it
		 * to be composited into a larger image.
		 */
		std::string CircuitBoard::get_svg(bool flipped, const ColorScheme& colors, const std::string& id_prefix) const {
			std::ostringstream ss;

			if (flipped) {
				for (auto it = layers.rbegin(); it != layers.rend(); ++it) {
					ss << (*it)->to_svg(colors, flipped, id_prefix);
				}
			}
			else {
				for (auto it = layers.begin(); it != layers.end(); ++it) {
					ss << (*it)->to_svg(colors, flipped, id_prefix);
				}
			}

			auto finish = svg::Layer(id_prefix + "finish");
			finish.add(flipped ? bottom_finish : top_finish, colors.finish);
			ss << finish;

			return ss.str();
		}

		void generateMaterial(std::ostringstream& stream, const std::string& type, const std::string& color, float transparency) {
			stream << "newmtl " << type << "\n";
			stream << "Kd " << color << "\n";
			stream << "d " << transparency << "\n";
			stream << "\n";
		}

		void CircuitBoard::generate_mtl_file(std::ostringstream& stream) const {
			generateMaterial(stream, "soldermask", "0.100 0.600 0.300", 0.600);
			generateMaterial(stream, "silkscreen", "0.899 0.899 0.899", 0.899);
			generateMaterial(stream, "finish", "0.699 0.699 0.699", 1.0);
			generateMaterial(stream, "substrate", "0.600 0.500 0.300", 1);
			generateMaterial(stream, "copper", "0.800 0.700 0.300", 1.0);
		}

		/**
		 * Renders the circuit board to an SVG.
		 */
		void CircuitBoard::write_svg(
			std::ostringstream& stream,
			bool flipped,
			double scale,
			const ColorScheme& colors
		) const {
			auto bounds = get_bounds();

			auto width = bounds.right - bounds.left + coord::Format::from_mm(20.0);
			auto height = bounds.top - bounds.bottom + coord::Format::from_mm(20.0);


			stream << "<svg viewBox=\"0 0 " << coord::Format::to_mm(width) << " " << coord::Format::to_mm(height) << "\"";
			stream << " width=\"" << coord::Format::to_mm(width) * scale << "\" height=\"" << coord::Format::to_mm(height) * scale << "\"";
			stream << " xmlns=\"http://www.w3.org/2000/svg\">\n";

			auto tx = coord::Format::from_mm(10.0) - (flipped ? -bounds.right : bounds.left);
			auto ty = coord::Format::from_mm(10.0) + bounds.top;

			stream << "<g transform=\"";
			stream << "translate(" << coord::Format::to_mm(tx) << " " << coord::Format::to_mm(ty) << ") ";
			stream << "scale(" << (flipped ? "-1" : "1") << " -1) ";
			stream << "\" filter=\"drop-shadow(0 0 1 rgba(0, 0, 0, 0.2))\">\n";

			stream << get_svg(flipped, colors);

			stream << "</g>\n";
			stream << "</svg>\n";
		}


		/**
		 * Generates a path that approximates a circle of the given size with the given
		 * center point.
		 */
		static void render_circle(coord::CPt center, coord::CInt diameter, coord::Path& output) {
			double epsilon = coord::Format().get_max_deviation();
			double r = diameter * 0.5;
			double x = (r > epsilon) ? (1.0 - epsilon / r) : 0.0;
			double th = std::acos(2.0 * x * x - 1.0) + 1e-3;
			auto n_vertices = (size_t)std::ceil(2.0 * M_PI / th);
			if (n_vertices < 3) n_vertices = 3;
			output.clear();
			output.reserve(n_vertices);
			for (size_t i = 0; i < n_vertices; i++) {
				auto a = 2.0 * M_PI * i / n_vertices;
				output.emplace_back(
					center.X + (coord::CInt)std::round(std::cos(a) * r),
					center.Y + (coord::CInt)std::round(std::sin(a) * r)
				);
			}
		}

		/**
		 * Adds named copper shapes to the given Wavefront OBJ file manager.
		 */
		static void render_copper(
			obj::ObjFile& obj,
			const netlist::PhysicalNetlist& netlist,
			const std::vector<std::pair<double, double>>& copper_z
		) {
			size_t name_counter = 1;
			for (const auto& net : netlist.get_nets()) {

				// Figure out a unique name for the copper object.
				std::string name;
				if (net->get_logical_nets().empty()) {
					name = "net_" + std::to_string(name_counter);
				}
				else {
					name = net->get_logical_nets().begin()->lock()->get_name() + "_" + std::to_string(name_counter);
				}
				name_counter++;
				auto& ob = obj.add_object(name, "copper");

				// Enumerate the vias connected to this net.
				struct Via {
					coord::CPt center;
					coord::Path inner;
					coord::Path outer;
					size_t lower_layer;
					size_t upper_layer;
				};
				std::vector<Via> vias;
				vias.reserve(net->get_vias().size());
				for (const auto& via : net->get_vias()) {
					auto center = via->get_coordinate();
					auto diameter = via->get_finished_hole_size();
					auto lower_layer = via->get_lower_layer(copper_z.size());
					auto upper_layer = via->get_upper_layer(copper_z.size());

					// Render the inner ring. This goes all the way from the bottom of
					// the lowest layer to the top of the upper layer.
					coord::Path inner;
					render_circle(center, diameter, inner);
					ob.add_ring(inner, copper_z.at(lower_layer).first, copper_z.at(upper_layer).second);

					// Render the outer rings. There is one for each layer that is
					// connected by the via, from the top of the lower of the two layers
					// to the bottom of the top of the two.
					coord::Path outer;
					render_circle(center, diameter + 2 * via->get_plating_thickness(), outer);
					for (size_t layer = lower_layer; layer < upper_layer; layer++) {
						ob.add_ring(outer, copper_z.at(layer).second, copper_z.at(layer + 1).first);
					}

					// Everything else consists of flat copper surfaces, along with
					// their own outline. We store the outer ring for this via in a
					// record to keep track of these, so we can punch holes in these
					// surfaces.
					vias.push_back({ center, std::move(inner), std::move(outer), lower_layer, upper_layer });

				}

				// Enumerate and render the planar copper shapes connected to this net.
				for (const auto& shape : net->get_shapes()) {
					auto zs = copper_z.at(shape->get_layer());

					// Add the rings.
					ob.add_ring(shape->get_outline(), zs.first, zs.second);
					for (const auto& path : shape->get_holes()) {
						ob.add_ring(path, zs.first, zs.second);
					}

					// Add the surfaces.
					size_t layer = shape->get_layer();
					for (int side = 0; side < 2; side++) {

						// Side 0 is the bottom of the sheet, side 1 is the top.
						double z = side ? zs.second : zs.first;

						// Figure out the holes in this shape, including those made by
						// vias.
						coord::Paths holes = shape->get_holes();
						for (const auto& via : vias) {
							if (layer < via.lower_layer) {
								// Via is above this side/layer.
								continue;
							}
							if (layer > via.upper_layer) {
								// Via is below this side/layer.
								continue;
							}
							if (!shape->contains(via.center)) {
								// Via is in a different part of the PCB in-plane.
								continue;
							}
							// Via punches through this side/layer, so we need to add an
							// extra hole for it.
							if ((layer == via.lower_layer && side == 0) || (layer == via.upper_layer && side == 1)) {
								holes.push_back(via.inner);
							}
							else {
								holes.push_back(via.outer);
							}
						}

						// Add the copper surface.
						ob.add_surface(shape->get_outline(), holes, z);

					}
				}
			}
		}

		/**
		 * Renders the circuit board to a Wavefront OBJ file. Optionally, a netlist
		 * can be supplied, of which the logical net names will then be used to
		 * name the copper objects.
		 */
		void CircuitBoard::write_obj(std::ostringstream& stream, const netlist::Netlist* netlist) const {
			obj::ObjFile obj;
			double z = 0.0;
			size_t index = 0;
			std::vector<std::pair<double, double>> copper_z;
			for (const auto& layer : layers) {
				layer->to_obj(obj, index++, z, "");
				if (std::dynamic_pointer_cast<CopperLayer>(layer)) {
					copper_z.emplace_back(z, z + layer->get_thickness());
				}
				z += layer->get_thickness();
			}
			if (netlist != nullptr) {
				render_copper(obj, netlist->get_physical_netlist(), copper_z);
			}
			else {
				render_copper(obj, get_physical_netlist(), copper_z);
			}
			obj.to_file(stream);
		}

		std::vector<std::string> ListFiles(const std::string& directory) {
			std::vector<std::string> files;

			for (const auto& entry : std::filesystem::recursive_directory_iterator(directory)) {
				if (entry.is_regular_file()) {
					files.push_back(std::filesystem::absolute(entry.path()).string());
				}
			}

			return files;
		}

		std::map<std::string, std::vector<std::string>> FileType::IdentifyGerberFiles(std::vector<pcb::GerberFile>& files) {

			std::map<std::string, std::vector<std::string>> charFiles;

			for each (auto file in files)
			{
				if (BoardFileType::Gerber == file.getFileType()) {
					BoardSide Side = BoardSide::Unknown;
					BoardLayer Layer = BoardLayer::Unknown;
					pcb::BoardSideAndLayer boardSideAndLayer(file.getFileName(), Side, Layer);
					boardSideAndLayer.DetermineBoardSideAndLayer();
					Side = boardSideAndLayer.getSide();
					Layer = boardSideAndLayer.getLayer();

					if (BoardLayer::Copper == Layer && BoardSide::Top == Side) {
						charFiles["topCopper"].push_back(file.getChar());
					}
					else if (BoardLayer::Copper == Layer && BoardSide::Bottom == Side) {
						charFiles["bottomCopper"].push_back(file.getChar());
					}
					else if (BoardLayer::SolderMask == Layer && BoardSide::Top == Side) {
						charFiles["topMask"].push_back(file.getChar());
					}
					else if (BoardLayer::SolderMask == Layer && BoardSide::Bottom == Side) {
						charFiles["bottomMask"].push_back(file.getChar());
					}
					else if (BoardLayer::Silk == Layer && BoardSide::Top == Side) {
						charFiles["topSilk"].push_back(file.getChar());
					}
					else if (BoardLayer::Silk == Layer && BoardSide::Bottom == Side) {
						charFiles["bottomSilk"].push_back(file.getChar());
					}
					else if (BoardLayer::Drill == Layer && BoardSide::Both == Side) {
						charFiles["drill"].push_back(file.getChar());
					}
					else if (BoardLayer::Mill == Layer && BoardSide::Both == Side) {
						charFiles["mill"].push_back(file.getChar());
					}
					else if (BoardLayer::Outline == Layer && BoardSide::Both == Side) {
						charFiles["outline"].push_back(file.getChar());
					}
				}

				else if (BoardFileType::Drill == file.getFileType()) {
					charFiles["drill"].push_back(file.getChar());

				}
			}

			return charFiles;
		}

		CircuitBoard CircuitBoard::LoadPCB(std::vector<pcb::GerberFile>& files) {
			FileType type;

				auto gerberFiles = type.IdentifyGerberFiles(files);
			if (gerberFiles.empty()) {
				throw std::runtime_error("No gerber file");
			}


			std::string basename = "C:/Users/Программист/Desktop/outputdir";
			std::string outline = gerberFiles["outline"].front();
			std::vector<std::string> drill= gerberFiles["drill"];

			std::string drill_nonplated;
			drill_nonplated.push_back('\0');
			std::string mill;
			mill.push_back('\0');
			double plating_thickness = 0.5 * pcb::COPPER_OZ;

			pcb::CircuitBoard board(basename, outline, drill, drill_nonplated, mill, plating_thickness);



			if (!gerberFiles["bottomMask"].empty()) {
				auto bottomMask = gerberFiles["bottomMask"].front();
				auto bottomSilk = gerberFiles["bottomSilk"].front();
				board.add_mask_layer(bottomMask, bottomSilk);
			}

			if (!gerberFiles["bottomCopper"].empty()) {
				auto bottomCopper = gerberFiles["bottomCopper"].front();
				board.add_copper_layer(bottomCopper, pcb::COPPER_OZ);
			}

			board.add_substrate_layer(1.5);

			if (!gerberFiles["topCopper"].empty()) {
				auto topCopper = gerberFiles["topCopper"].front();
				board.add_copper_layer(topCopper, pcb::COPPER_OZ);
			}

			if (!gerberFiles["topMask"].empty()) {
				auto topMask = gerberFiles["topMask"].front();
				auto topSilk = gerberFiles["topSilk"].front();
				board.add_mask_layer(topMask, topSilk);
			}

			board.add_surface_finish();
			gerberFiles.clear();


			return board;
		}

		void BoardSideAndLayer::DetermineBoardSideAndLayer() {
			Side = BoardSide::Unknown;
			Layer = BoardLayer::Unknown;
			std::string gerberfile(GerberFile.begin(), GerberFile.end());
			std::string filename = gerberfile.substr(gerberfile.find_last_of("/\\") + 1);
			std::string ext = filename.substr(filename.find_last_of('.') + 1);
			std::transform(ext.begin(), ext.end(), ext.begin(), ::tolower);

			if (ext == "art") {
				std::string baseName = filename.substr(0, filename.find_last_of('.'));
				std::transform(baseName.begin(), baseName.end(), baseName.begin(), ::toupper);

				if (baseName == "PMT") { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
				else if (baseName == "PMB") { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
				else if (baseName == "TOP") { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
				else if (baseName == "BOTTOM") { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
				else if (baseName == "SMBOT") { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
				else if (baseName == "SMTOP") { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
				else if (baseName == "SSBOT") { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
				else if (baseName == "SSTOP") { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
				else if (baseName == "DRILLING") { Side = BoardSide::Both; Layer = BoardLayer::Drill; }
			}
			else if (ext == "slices") { Side = BoardSide::Both; Layer = BoardLayer::Utility; }
			else if (ext == "copper_bottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
			else if (ext == "copper_top") { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
			else if (ext == "silk_bottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
			else if (ext == "silk_top") { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
			else if (ext == "paste_bottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
			else if (ext == "paste_top") { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
			else if (ext == "soldermask_bottom") { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
			else if (ext == "soldermask_top") { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
			else if (ext == "drill_both") { Side = BoardSide::Both; Layer = BoardLayer::Drill; }
			else if (ext == "outline_both") { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
			else if (ext == "png") {
				Side = BoardSide::Both;
				Layer = BoardLayer::Silk;
			}
			else if (ext == "assemblytop") {
				Layer = BoardLayer::Assembly;
				Side = BoardSide::Top;
			}
			else if (ext == "assemblybottom") {
				Layer = BoardLayer::Assembly;
				Side = BoardSide::Bottom;
			}
			else if (ext == "gbr") {
				std::string baseName = filename.substr(0, filename.find_last_of('.'));
				std::transform(baseName.begin(), baseName.end(), baseName.begin(), ::tolower);

				if (baseName == "profile" || baseName == "boardoutline" || baseName == "outline" || baseName == "board") {
					Side = BoardSide::Both;
					Layer = BoardLayer::Outline;
				}
				else if (baseName == "copper_bottom" || baseName == "bottom") {
					Side = BoardSide::Bottom;
					Layer = BoardLayer::Copper;
				}
				else if (baseName == "soldermask_bottom" || baseName == "bottommask") {
					Side = BoardSide::Bottom;
					Layer = BoardLayer::SolderMask;
				}
				else if (baseName == "solderpaste_bottom" || baseName == "bottompaste") {
					Side = BoardSide::Bottom;
					Layer = BoardLayer::Paste;
				}
				else if (baseName == "silkscreen_bottom" || baseName == "bottomsilk") {
					Side = BoardSide::Bottom;
					Layer = BoardLayer::Silk;
				}
				else if (baseName == "copper_top" || baseName == "top") {
					Side = BoardSide::Top;
					Layer = BoardLayer::Copper;
				}
				else if (baseName == "soldermask_top" || baseName == "topmask") {
					Side = BoardSide::Top;
					Layer = BoardLayer::SolderMask;
				}
				else if (baseName == "solderpaste_top" || baseName == "toppaste") {
					Side = BoardSide::Top;
					Layer = BoardLayer::Paste;
				}
				else if (baseName == "silkscreen_top" || baseName == "topsilk") {
					Side = BoardSide::Top;
					Layer = BoardLayer::Silk;
				}
				else if (baseName == "inner1") {
					Side = BoardSide::Internal1;
					Layer = BoardLayer::Copper;
				}
				else if (baseName == "inner2") {
					Side = BoardSide::Internal2;
					Layer = BoardLayer::Copper;
				}
				else {
					std::string gerberfile(GerberFile.begin(), GerberFile.end());

					std::string lcase = gerberfile;
					std::transform(lcase.begin(), lcase.end(), lcase.begin(), ::tolower);
					if (lcase.find("board outline") != std::string::npos) { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
					if (lcase.find("copper bottom") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
					if (lcase.find("silkscreen bottom") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
					if (lcase.find("copper top") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
					if (lcase.find("silkscreen top") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
					if (lcase.find("solder mask bottom") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
					if (lcase.find("solder mask top") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
					if (lcase.find("drill-copper top-copper bottom") != std::string::npos) { Side = BoardSide::Both; Layer = BoardLayer::Drill; }
					if (lcase.find("outline") != std::string::npos) { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
					if (lcase.find("-edge_cuts") != std::string::npos) { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
					if (lcase.find("-b_cu") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
					if (lcase.find("-f_cu") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
					if (lcase.find("-b_silks") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
					if (lcase.find("-f_silks") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
					if (lcase.find("-b_mask") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
					if (lcase.find("-f_mask") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
					if (lcase.find("-b_paste") != std::string::npos) { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
					if (lcase.find("-f_paste") != std::string::npos) { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
				}
			}
			else if (ext == "ger") {
				std::string gerberfileGer(GerberFile.begin(), GerberFile.end());

				std::string l = gerberfileGer;
				std::transform(l.begin(), l.end(), l.begin(), ::tolower);
				std::vector<BoardSet> bs = {
					{ ".topsoldermask", BoardSide::Top, BoardLayer::SolderMask },
					{ ".topsilkscreen", BoardSide::Top, BoardLayer::Silk },
					{ ".toplayer", BoardSide::Top, BoardLayer::Copper },
					{ ".tcream", BoardSide::Top, BoardLayer::Paste },
					{ ".boardoutline", BoardSide::Both, BoardLayer::Outline },
					{ ".bcream", BoardSide::Bottom, BoardLayer::Paste },
					{ ".bottomsoldermask", BoardSide::Bottom, BoardLayer::SolderMask },
					{ ".bottomsilkscreen", BoardSide::Bottom, BoardLayer::Silk },
					{ ".bottomlayer", BoardSide::Bottom, BoardLayer::Copper },
					{ ".internalplane1", BoardSide::Internal1, BoardLayer::Copper },
					{ ".internalplane2", BoardSide::Internal2, BoardLayer::Copper }
				};

				for (const auto& a : bs) {
					if (l.find(a.name) != std::string::npos) {
						Side = a.side;
						Layer = a.layer;
						break;
					}
				}
			}
			else if (ext == "gml") { Side = BoardSide::Both; Layer = BoardLayer::Mill; }
			else if (ext == "fabrd" || ext == "oln" || ext == "gko" || ext == "gm1") { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
			else if (ext == "l2" || ext == "g1l" || ext == "gl1" || ext == "g1") { Side = BoardSide::Internal1; Layer = BoardLayer::Copper; }
			else if (ext == "adtop") { Side = BoardSide::Top; Layer = BoardLayer::Assembly; }
			else if (ext == "adbottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Assembly; }
			else if (ext == "notes") { Side = BoardSide::Both; Layer = BoardLayer::Notes; }
			else if (ext == "l3" || ext == "gl2" || ext == "g2l" || ext == "g2") { Side = BoardSide::Internal2; Layer = BoardLayer::Copper; }
			else if (ext == "gl3" || ext == "g3l" || ext == "g3") { Side = BoardSide::Internal3; Layer = BoardLayer::Copper; }
			else if (ext == "gl4" || ext == "g4l" || ext == "g4") { Side = BoardSide::Internal4; Layer = BoardLayer::Copper; }
			else if (ext == "gl5" || ext == "g5l" || ext == "g5") { Side = BoardSide::Internal5; Layer = BoardLayer::Copper; }
			else if (ext == "gl6" || ext == "g6l" || ext == "g6") { Side = BoardSide::Internal6; Layer = BoardLayer::Copper; }
			else if (ext == "gl7" || ext == "g7l" || ext == "g7") { Side = BoardSide::Internal7; Layer = BoardLayer::Copper; }
			else if (ext == "gl8" || ext == "g8l" || ext == "g8") { Side = BoardSide::Internal8; Layer = BoardLayer::Copper; }
			else if (ext == "gl9" || ext == "g9l" || ext == "g9") { Side = BoardSide::Internal9; Layer = BoardLayer::Copper; }
			else if (ext == "gl10" || ext == "g10l" || ext == "g10") { Side = BoardSide::Internal10; Layer = BoardLayer::Copper; }
			else if (ext == "gl11" || ext == "g11l" || ext == "g11") { Side = BoardSide::Internal11; Layer = BoardLayer::Copper; }
			else if (ext == "gl12" || ext == "g12l" || ext == "g12") { Side = BoardSide::Internal12; Layer = BoardLayer::Copper; }
			else if (ext == "gl13" || ext == "g13l" || ext == "g13") { Side = BoardSide::Internal13; Layer = BoardLayer::Copper; }
			else if (ext == "gl14" || ext == "g14l" || ext == "g14") { Side = BoardSide::Internal14; Layer = BoardLayer::Copper; }
			else if (ext == "gl15" || ext == "g15l" || ext == "g15") { Side = BoardSide::Internal15; Layer = BoardLayer::Copper; }
			else if (ext == "gl16" || ext == "g16l" || ext == "g16") { Side = BoardSide::Internal16; Layer = BoardLayer::Copper; }
			else if (ext == "gl17" || ext == "g17l" || ext == "g17") { Side = BoardSide::Internal17; Layer = BoardLayer::Copper; }
			else if (ext == "gl18" || ext == "g18l" || ext == "g18") { Side = BoardSide::Internal18; Layer = BoardLayer::Copper; }
			else if (ext == "gl19" || ext == "g19l" || ext == "g19") { Side = BoardSide::Internal19; Layer = BoardLayer::Copper; }
			else if (ext == "gl20" || ext == "g20l" || ext == "g20") { Side = BoardSide::Internal20; Layer = BoardLayer::Copper; }
			else if (ext == "l4" || ext == "gbl" || ext == "l2m") { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
			else if (ext == "l1" || ext == "l1m" || ext == "gtl") { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
			else if (ext == "gbp" || ext == "spbottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
			else if (ext == "gtp" || ext == "sptop") { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
			else if (ext == "gbo" || ext == "ss2" || ext == "ssbottom") { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
			else if (ext == "gto" || ext == "ss1" || ext == "sstop") { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
			else if (ext == "gbs" || ext == "sm2" || ext == "smbottom") { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
			else if (ext == "gts" || ext == "sm1" || ext == "smtop") { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
			else if (ext == "outline" || ext == "gb3") { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
			else if (ext == "gt3") { Side = BoardSide::Both; Layer = BoardLayer::Outline; }
			else if (ext == "top") { Side = BoardSide::Top; Layer = BoardLayer::Copper; }
			else if (ext == "bottom" || ext == "bot") { Side = BoardSide::Bottom; Layer = BoardLayer::Copper; }
			else if (ext == "smb") { Side = BoardSide::Bottom; Layer = BoardLayer::SolderMask; }
			else if (ext == "smt") { Side = BoardSide::Top; Layer = BoardLayer::SolderMask; }
			else if (ext == "slk" || ext == "sst") { Side = BoardSide::Top; Layer = BoardLayer::Silk; }
			else if (ext == "bsk" || ext == "ssb") { Side = BoardSide::Bottom; Layer = BoardLayer::Silk; }
			else if (ext == "spt") { Side = BoardSide::Top; Layer = BoardLayer::Paste; }
			else if (ext == "spb") { Side = BoardSide::Bottom; Layer = BoardLayer::Paste; }
			else if (ext == "drill_top_bottom" || ext == "drl" || ext == "drill" || ext == "drillnpt" || ext == "rou" || ext == "sco") {
				Side = BoardSide::Both;
				Layer = BoardLayer::Drill;
			}
		}

		BoardFileType FileType::FindFileTypeFromStream(std::istream& file, const std::string& filename) {
			std::string lower_filename = filename;
			std::transform(lower_filename.begin(), lower_filename.end(), lower_filename.begin(), ::tolower);

			// Используем std::set для проверки расширений
			static const std::set<std::string> unsupported = { "config", "exe", "dll", "png", "zip", "gif", "jpeg", "doc", "docx", "jpg", "bmp" };
			std::string ext = filename.substr(filename.find_last_of(".") + 1);

			if (unsupported.find(ext) != unsupported.end()) {
				return BoardFileType::Unsupported;
			}

			std::string line;
			while (std::getline(file, line)) {
				if (line.find("%FS") != std::string::npos) return BoardFileType::Gerber;
				if (line.find("M48") != std::string::npos) return BoardFileType::Drill;
			}

			return BoardFileType::Unsupported;
		}

	} // namespace pcb
} // namespace gerbertools



