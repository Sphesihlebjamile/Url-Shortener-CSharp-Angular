import { Routes } from '@angular/router';
import { NotFound } from './not-found';
import { Home } from './pages/home/home';

export const routes: Routes = [
  { path: '', component: Home },
  { path: '404', component: NotFound },
  { path: '**', redirectTo: '/404' },
];
