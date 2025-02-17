import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import axios from 'axios';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./auth.css']
})

export class RegisterComponent {
  registerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    });
  }

  async register() {
    if (this.registerForm.valid) {
      const formValue = this.registerForm.value;
      if (formValue.password !== formValue.confirmPassword) {
        alert('Passwords do not match!');
        return;
      }
      
      const params = new URLSearchParams();
      params.append('Username', formValue.username);
      params.append('Email', formValue.email);
      params.append('Password', formValue.password);

      const registerReq = await axios.post(environment.apiUrl+"/api/auth/register", params);

      if(registerReq.status === 200) {
        this.router.navigate(['/auth/login']);
      } else {
        alert('Registration failed!');
      }
    }
  }

  close() {
    this.router.navigate(['/']);
  }
}