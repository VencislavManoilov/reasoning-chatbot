import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import axios from 'axios';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./auth.css'],
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  async login() {
    if (this.loginForm.valid) {
      const formValue = this.loginForm.value;

      const params = new URLSearchParams();
      params.append('Email', formValue.email);
      params.append('Password', formValue.password);

      try {
        const loginReq = await axios.post(
          environment.apiUrl+"/api/auth/login",
          params
        );

        localStorage.setItem('token', loginReq.data.token);
        alert('Login successful!');
        window.location.reload();
      } catch(error) {
        alert('Registration failed!');
      }
    }
  }

  close() {
    this.router.navigate(['/']);
  }
}
