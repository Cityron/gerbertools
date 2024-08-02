#pragma once

#include <tuple>
#include "color.hpp"

namespace gerbertools {

    using Color = std::tuple<float, float, float, float>;

    inline Color color_to_tuple(const color::Color& c) {
        return { c.r, c.g, c.b, c.a };
    }

    inline color::Color tuple_to_color(const Color& c) {
        return { std::get<0>(c), std::get<1>(c), std::get<2>(c), std::get<3>(c) };
    }

}