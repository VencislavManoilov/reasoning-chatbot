import { Component } from '@angular/core';

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.component.html',
  styleUrls: ['register.component.css']
})

export class RegisterComponent {
  username: string = '';
  password: string = '';
  confirmPassword: string = '';

  register() {
    if (this.password !== this.confirmPassword) {
      console.error('Passwords do not match!');
      return;
    }

    console.log('Registering user:', this.username);
  }
}