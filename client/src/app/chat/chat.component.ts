import { Component } from '@angular/core';
import axios from 'axios';
import { SidebarComponent } from "./sidebar/sidebar.component";

@Component({
  selector: 'app-chat',
  imports: [SidebarComponent],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent {
  title = 'chat';
  chats: any[];

  constructor() {
    this.chats = [];
  }

  async GetChats(): Promise<void> {
    try {
      const response = await axios.get('http://localhost:5010/api/chat/list', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
      })

      this.chats = response.data;
    } catch(error) {
      try {
        const profile = await axios.get('http://localhost:5010/api/auth/profile', {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });

        alert('Error getting chat history');
      } catch(error) {
        localStorage.removeItem('token');
        window.location.href = '/';
      }
    }
  }

  ngOnInit() {
    this.GetChats();
  }
}
