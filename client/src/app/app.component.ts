import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Router } from '@angular/router';
import axios from 'axios';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client';

  constructor(private router: Router) {}

  openRegister() {
    this.router.navigate(['/auth/register']);
  }

  openLogin() {
    this.router.navigate(['/auth/login']);
  }

  async checkSession(): Promise<void> {
    const sessionId = localStorage.getItem('sessionId');
    if(!sessionId) {
      return;
    }

    const request = await axios.get('http://localhost:5010/api/auth/profile', { withCredentials: true });

    if(request.status !== 200) {
      localStorage.removeItem('sessionId');
    } else {
      this.router.navigate(['/profile'], { state: { data: request.data } });
    }
  }

  ngOnInit() {
    this.checkSession();
  }
}