import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Router } from '@angular/router';

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
    const token = localStorage.getItem('token');
    if(!token) {
      return;
    }

    this.router.navigate(['/chat']);
  }

  ngOnInit() {
    this.checkSession();
  }
}