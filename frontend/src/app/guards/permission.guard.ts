import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '@services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class PermissionGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    const requiredPermission = route.data['requiredPermission'] as string;
    const requiredPermissions = route.data['requiredPermissions'] as string[];

    if (requiredPermissions && requiredPermissions.length > 0) {
      return requiredPermissions.every((permission) =>
        this.authService.hasPermission(permission)
      );
    }

    if (
      !requiredPermission ||
      this.authService.hasPermission(requiredPermission)
    ) {
      return true;
    }

    return this.router.createUrlTree(['/unauthorized']);
  }
}
