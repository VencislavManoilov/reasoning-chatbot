import { Routes } from '@angular/router';
import { RegisterComponent } from './auth/register.component';
import { LoginComponent } from './auth/login.component';
import { ProfileComponent } from './profile/profile.component';

export const routes: Routes = [
    { path: 'auth/register', component: RegisterComponent },
    { path: 'auth/login', component: LoginComponent },
    { path: 'profile', component: ProfileComponent }
];
