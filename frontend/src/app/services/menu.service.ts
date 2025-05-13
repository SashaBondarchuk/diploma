import { Injectable } from '@angular/core';
import { type Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import type { MenuItem } from 'primeng/api';
import { AuthService } from './auth.service';
import type { MenuItemModel } from '@models/menu-item.model';
import { ALL_MENU_ITEMS } from '@app/constants/menu-items';

@Injectable({
  providedIn: 'root',
})
export class MenuService {
  constructor(private authService: AuthService) {}

  getMenuItemsForCurrentUser(
    onClick: (item: MenuItemModel) => void
  ): Observable<MenuItem[]> {
    return this.authService.currentUser$.pipe(
      map((user) => {
        if (!user) return [];
        const menuItems = JSON.parse(JSON.stringify(ALL_MENU_ITEMS));
        const filteredItems = this.filterMenuItemsByPermissions(
          menuItems,
          user.permissions
        );
        return this.transformToMenuItems(filteredItems, onClick);
      }),
      catchError((error) => {
        console.error('Error processing menu items:', error);
        return of([]);
      })
    );
  }

  private filterMenuItemsByPermissions(
    items: MenuItemModel[],
    permissions: string[]
  ): MenuItemModel[] {
    return items.filter((item) => {
      const hasPermission = item.permissions?.every((permission) =>
        permissions.includes(permission)
      );

      if (item.children && item.children.length > 0) {
        item.children = this.filterMenuItemsByPermissions(
          item.children,
          permissions
        );
      }

      return hasPermission || (item.children && item.children.length > 0);
    });
  }

  private transformToMenuItems(
    items: MenuItemModel[],
    onClick: (item: MenuItemModel) => void
  ): MenuItem[] {
    return items.map((item) => {
      const hasChildren = !!item.children && item.children.length > 0;

      return {
        label: item.label,
        icon: item.icon,
        styleClass: hasChildren ? undefined : 'router-link-active',
        expanded: hasChildren,
        routerLinkActiveOptions: { exact: true },
        routerLink: !hasChildren && item.route ? [item.route] : undefined,
        items: hasChildren
          ? this.transformToMenuItems(item.children!, onClick)
          : undefined,
        command: !hasChildren ? () => onClick(item) : undefined,
      };
    });
  }
}
