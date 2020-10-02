"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.WidgetSettings = void 0;
var axis_settings_model_1 = require("./axis-settings.model");
var WidgetSettings = /** @class */ (function () {
    function WidgetSettings() {
        this.xAxisSettings = new axis_settings_model_1.AxisSettings(axis_settings_model_1.AxisType.X);
        this.yLeftAxisSettings = new axis_settings_model_1.AxisSettings(axis_settings_model_1.AxisType.YLeft);
        this.yRightAxisSettings = new axis_settings_model_1.AxisSettings(axis_settings_model_1.AxisType.YRight);
    }
    return WidgetSettings;
}());
exports.WidgetSettings = WidgetSettings;
//# sourceMappingURL=widget-settings.model.js.map