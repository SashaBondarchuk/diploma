import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { EmployeeClassService } from '@app/services/employee-class.service';
import { EmployeeClassModel } from '@models/employee-class.model';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-employee-classes',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './employee-classes.component.html',
  styleUrls: ['./employee-classes.component.scss']
})
export class EmployeeClassesComponent implements OnInit {
  employeeClasses: EmployeeClassModel[] = [];
  loading: boolean = true;

  constructor(private employeeClassService: EmployeeClassService) {}

  ngOnInit() {
    this.loadEmployeeClasses();
  }

  loadEmployeeClasses() {
    this.loading = true;
    this.employeeClassService.getAllEmployeeClasses().subscribe({
      next: (classes) => {
        this.employeeClasses = classes;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading employee classes:', error);
        this.loading = false;
      }
    });
  }

  getClassStyleClassname(className: string): string {
    return this.employeeClassService.getClassStyleClassname(className);
  }
} 