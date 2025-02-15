import { Component } from '@angular/core';
import axios from 'axios';
import { SidebarComponent } from "./sidebar/sidebar.component";
import { Router, ActivatedRoute } from '@angular/router';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-chat',
  imports: [SidebarComponent, NgFor, NgIf],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent {
  title = 'chat';
  chats: any[] = [];
  chatId: any;
  chat: any = [];

  constructor(private router: Router, private route: ActivatedRoute) {}

  async GetChat(id: number): Promise<void> {
    try {
      const response = await axios.get(`http://localhost:5010/api/chat/view`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        params: {
          ChatId: id
        }
      });

      this.chat = response.data.messages;
    } catch(error) {
      alert('Error getting chat');
      this.router.navigate(['/chat']);
    }
  }

  async GetChats(): Promise<void> {
    try {
      const response = await axios.get('http://localhost:5010/api/chat/list', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
      });

      this.chats = response.data;
    } catch(error) {
      try {
        await axios.get('http://localhost:5010/api/auth/profile', {
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

  openProfile(event: Event) {
    event.preventDefault();
    this.router.navigate(['/profile']);
  }

  ngOnInit() {
    this.GetChats();

    // Listen for changes in the chat ID
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.chatId = id;
        this.GetChat(parseInt(id));
      } else {
        this.chat = [];
        this.chatId = null;
      }
    });
  }
}
