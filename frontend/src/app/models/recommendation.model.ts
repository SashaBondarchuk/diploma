import type { EmployeePartial } from './employee.model';

export interface Recommendation {
  id: number;
  employeeId: number;
  recommendationText: string;
  createdAt: string;
  isVisibleToEmployee: boolean;
  employee: EmployeePartial;
}

export interface UpdateRecommendationRequest {
  employeeId: number;
  recommendationText: string;
  isVisibleToEmployee: boolean;
}

