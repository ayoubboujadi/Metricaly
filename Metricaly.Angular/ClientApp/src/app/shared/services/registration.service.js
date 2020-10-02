"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.RegistrationService = void 0;
var core_1 = require("@angular/core");
var RegistrationService = /** @class */ (function () {
    function RegistrationService(http) {
        this.http = http;
    }
    RegistrationService.prototype.register = function (user) {
        return this.http.post("https://localhost:44344/api/auth/signup", user);
        //return this.http.post(`${config.apiUrl}/users/register`, user);
    };
    RegistrationService = __decorate([
        core_1.Injectable({ providedIn: 'root' })
    ], RegistrationService);
    return RegistrationService;
}());
exports.RegistrationService = RegistrationService;
//# sourceMappingURL=registration.service.js.map