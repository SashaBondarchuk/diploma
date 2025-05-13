export interface MenuItemModel {
  id: number;
  route?: string;
  icon?: string;
  label: string;
  permissions?: string[];
  children?: MenuItemModel[];
}
