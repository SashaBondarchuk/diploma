import { Component, type OnInit, type OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '@components/header/header.component';
import { SidebarComponent } from '@components/sidebar/sidebar.component';
import { SharedModule } from '@shared/shared.module';
import { MenuService } from '@services/menu.service';
import { AuthService } from '@services/auth.service';
import type { Employee } from '@models/employee.model';
import type { JwtPayload } from '@models/auth.model';
import type { MenuItem } from 'primeng/api';
import { filter, Subject, takeUntil, type Subscription } from 'rxjs';
import { NavigationEnd, Router } from '@angular/router';
import { MenuItemModel } from '@app/models/menu-item.model';
import { MessageService } from 'primeng/api';
import { ToastState } from '@app/shared/toast-state';
import { SPECIAL_ROUTES } from '@app/constants/menu-items';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, HeaderComponent, SidebarComponent, SharedModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit, OnDestroy {
  selectedMenuItem: string | null | undefined = null;
  currentUser: JwtPayload | null = null;
  currentEmployee: Employee | null = null;
  menuItems: MenuItem[] = [];
  loading = true;

  private subscriptions: Subscription[] = [];
  private destroy$ = new Subject<void>();
  private routerEventsSubscription: Subscription | null = null;

  constructor(
    private menuService: MenuService,
    private authService: AuthService,
    private router: Router,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {   
    this.loadCurrentUser();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
    if (this.routerEventsSubscription) {
      this.routerEventsSubscription.unsubscribe();
    }
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadCurrentUser(): void {
    this.authService.currentUser$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (user) => {
        this.currentUser = user;
        this.authService.setCurrentEmployee();
        this.loadMenuItems();
      },
      error: (error) => {
        console.error('Error loading current user:', error);
      },
    });

    this.authService.currentEmployee$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (employee) => {
        if (employee) {
          this.currentEmployee = employee;
        }
      },
    });
  }

  loadMenuItems(): void {
    const onClickCallback = (item: MenuItemModel) => {
      if (!item.children) {
        this.selectedMenuItem = item.label;
        this.router.navigate([item.route]);
      }
    };

    const menuSub = this.menuService
      .getMenuItemsForCurrentUser(onClickCallback)
      .subscribe({
        next: (menuItems) => {
          this.menuItems = menuItems;
          this.setupRouteTracking();
        },
        error: (error) => {
          console.error('Error loading menu items:', error);
        },
      });

    this.subscriptions.push(menuSub);
  }

  logout(): void {
    this.ngOnDestroy();
    this.authService.logout();
  }

  private setupRouteTracking(): void {
    this.handleRouteChange();

    this.routerEventsSubscription = this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.handleRouteChange();
      });
  }

  private handleRouteChange(): void {
    const state = this.router.getCurrentNavigation()?.extras
      .state as ToastState;
    if (state?.toastMessage) {
      this.messageService.add(state.toastMessage);
    }

    const currentUrl = this.router.url;
    const specialRouteEntry = Object.entries(SPECIAL_ROUTES).find(([key]) =>
      currentUrl.startsWith(key)
    );
    if (specialRouteEntry) {
      this.selectedMenuItem = specialRouteEntry[1];
      return;
    }

    const matchingMenuItem = this.findMenuItemByRoute(currentUrl);
    if (matchingMenuItem) {
      this.setSelectedMenuItem(matchingMenuItem);
      return;
    }

    this.setFirstMenuItemSelected();
  }

  private findMenuItemByRoute(route: string): MenuItem | null {
    for (const item of this.menuItems) {
      if (item.routerLink && item.routerLink[0] === route) {
        return item;
      }
      if (item.items) {
        const childItem = item.items.find(
          (child) => child.routerLink && child.routerLink[0] === route
        );
        if (childItem) {
          return childItem;
        }
      }
    }
    return null;
  }

  private setFirstMenuItemSelected(): void {
    const firstMenuItem = this.getFirstMenuItem(this.menuItems);
    this.setSelectedMenuItem(firstMenuItem);
  }

  private getFirstMenuItem(menuItems: MenuItem[]): MenuItem {
    const doesFirstItemHaveChildren =
      menuItems[0].items && menuItems[0].items.length > 0;

    return doesFirstItemHaveChildren ? menuItems[0].items![0] : menuItems[0];
  }

  private setSelectedMenuItem(menuItem: MenuItem): void {
    this.selectedMenuItem = menuItem.label;
    this.router.navigate(menuItem.routerLink);
  }
}
