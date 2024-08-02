#ifndef BOARD_TYPES_H
#define BOARD_TYPES_H

#include <string>

enum class BoardLayer {
    Notes,
    Assembly,
    Utility,
    Carbon,
    Silk,
    Paste,
    SolderMask,
    Copper,
    Drill,
    Outline,
    Mill,
    Unknown,
    Any
};

enum class BoardSide {
    Top,
    Bottom,
    Internal,
    Both,
    Unknown,
    Internal1,
    Internal2,
    Internal3,
    Internal4,
    Internal5,
    Internal6,
    Internal7,
    Internal8,
    Internal9,
    Internal10,
    Internal11,
    Internal12,
    Internal13,
    Internal14,
    Internal15,
    Internal16,
    Internal17,
    Internal18,
    Internal19,
    Internal20,
    Either
};

enum class InterpolationMode {
    Linear,
    ClockWise,
    CounterClockwise
};

enum class Quadrant {
    xposypos,
    xnegypos,
    xnegyneg,
    xposyneg
};

enum class GerberQuadrantMode {
    Multi,
    Single
};

enum class BoardFileType {
    Gerber,
    Drill,
    Unsupported
};

struct BoardSet {
    std::string name;
    BoardSide side;
    BoardLayer layer;
};

#endif // BOARD_TYPES_H