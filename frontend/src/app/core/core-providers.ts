import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { Provider } from '@angular/core';
import { AuthInterceptor } from '@interceptors/auth.interceptor';
import { ApiService } from '@services/api.service';
import { AuthService } from '@services/auth.service';
import { MenuService } from '@services/menu.service';
import { UserService } from '@services/user.service';

export const CORE_PROVIDERS: Provider[] = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ApiService,
  AuthService,
  MenuService,
  UserService,
];
