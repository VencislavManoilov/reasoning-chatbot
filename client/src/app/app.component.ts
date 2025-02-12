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

  async checkSession(): Promise<void> {
    const sessionId = localStorage.getItem('sessionId');
    if(!sessionId) {
      return;
    }

    try {
      const request = await axios.get('http://localhost:5010/api/auth/profile', { withCredentials: true });
      this.router.navigate(['/profile'], { state: { data: request.data } });
    } catch(error) {
      localStorage.removeItem('sessionId');
    }
  }

  ngOnInit() {
    this.checkSession();
  }
}