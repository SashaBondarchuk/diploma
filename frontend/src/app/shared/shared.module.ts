import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// PrimeNG Modules
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { TooltipModule } from 'primeng/tooltip';
import { PanelMenuModule } from 'primeng/panelmenu';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { DividerModule } from 'primeng/divider';
import { PanelModule } from 'primeng/panel';
import { SkeletonModule } from 'primeng/skeleton';
import { TabViewModule } from 'primeng/tabview';
import { DatePickerModule } from 'primeng/datepicker';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';
import { TabsModule } from 'primeng/tabs';
import { StepperModule } from 'primeng/stepper';
import { FieldsetModule } from 'primeng/fieldset';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    ButtonModule,
    AvatarModule,
    TooltipModule,
    PanelMenuModule,
    PanelModule,
    ProgressSpinnerModule,
    CardModule,
    TableModule,
    DialogModule,
    InputTextModule,
    DropdownModule,
    ToastModule,
    ConfirmDialogModule,
    DividerModule,
    SkeletonModule,
    TabViewModule,
    DatePickerModule,
    CheckboxModule,
    InputNumberModule,
    TextareaModule,
    TabsModule,
    StepperModule,
    FieldsetModule,
  ],
  exports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    ButtonModule,
    AvatarModule,
    TooltipModule,
    PanelMenuModule,
    ProgressSpinnerModule,
    CardModule,
    TableModule,
    DialogModule,
    InputTextModule,
    DropdownModule,
    ToastModule,
    ConfirmDialogModule,
    DividerModule,
    PanelModule,
    SkeletonModule,
    TabViewModule,
    DatePickerModule,
    CheckboxModule,
    InputNumberModule,
    TextareaModule,
    TabsModule,
    StepperModule,
    FieldsetModule,
  ],
  providers: [MessageService, ConfirmationService],
})
export class SharedModule {}
