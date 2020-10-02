"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AxisSettings = exports.AxisType = void 0;
var AxisType;
(function (AxisType) {
    AxisType[AxisType["X"] = 0] = "X";
    AxisType[AxisType["YLeft"] = 1] = "YLeft";
    AxisType[AxisType["YRight"] = 2] = "YRight";
})(AxisType = exports.AxisType || (exports.AxisType = {}));
var AxisSettings = /** @class */ (function () {
    function AxisSettings(type) {
        this.type = type;
    }
    return AxisSettings;
}());
exports.AxisSettings = AxisSettings;
//# sourceMappingURL=axis-settings.model.js.map