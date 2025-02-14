import { NgFor } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-chat-sidebar',
  imports: [NgFor],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  @Input() data: any;

  constructor(private router: Router) {}

  openChat(id: number): void {
    this.router.navigate([`/chat/${id}`]);
  }
}
