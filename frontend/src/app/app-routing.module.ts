import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { SalesComponent } from './pages/sales/sales.component';
import { ProductsComponent } from './pages/products/products.component';
import { CustomersComponent } from './pages/customers/customers.component';
import { GeographyComponent } from './pages/geography/geography.component';
import { EmployeesComponent } from './pages/employees/employees.component';
import { DimensionsComponent } from './pages/dimensions/dimensions.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'dashboard', component: DashboardComponent, title: 'SalesPulse | Dashboard' },
  { path: 'sales', component: SalesComponent, title: 'SalesPulse | Sales Analytics' },
  { path: 'products', component: ProductsComponent, title: 'SalesPulse | Product Analytics' },
  { path: 'customers', component: CustomersComponent, title: 'SalesPulse | Customer Analytics' },
  { path: 'geography', component: GeographyComponent, title: 'SalesPulse | Geography Analytics' },
  { path: 'employees', component: EmployeesComponent, title: 'SalesPulse | Employee Analytics' },
  { path: 'dimensions', component: DimensionsComponent, title: 'SalesPulse | Filters & Members' },
  { path: '**', redirectTo: 'dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
