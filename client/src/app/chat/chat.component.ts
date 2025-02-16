import { Component } from '@angular/core';
import axios from 'axios';
import { SidebarComponent } from "./sidebar/sidebar.component";
import { Router, ActivatedRoute } from '@angular/router';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  imports: [SidebarComponent, NgFor, NgIf, NgClass, FormsModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent {
  title = 'chat';
  chats: any[] = [];
  chatId: any;
  chat: any = [];
  messageContent: string = '';

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

  async sendMessage(): Promise<void> {
    if (!this.messageContent.trim()) return;
  
    try {
      if(this.chat.length === 0) {
        this.chat = [{ role: "user", content: this.messageContent }];
      } else {
        this.chat.push({ role: "user", content: this.messageContent });
      }
  
      const params = new URLSearchParams();
      if(this.chatId) {
        params.append('ChatId', this.chatId);
      }
      params.append('Message', this.messageContent);
      this.messageContent = '';
  
      const response = await axios.post('http://localhost:5010/api/chat/send', params, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        responseType: 'stream'
      });
  
      // const reader = response.data.getReader();
      // const decoder = new TextDecoder('utf-8');
      // let assistantMessage = { role: "assistant", content: "" };
      // this.chat.push(assistantMessage);
  
      // while (true) {
      //   const { done, value } = await reader.read();
      //   if (done) break;
      //   assistantMessage.content += decoder.decode(value, { stream: true });
      //   this.updateChatView();
      // }

      this.chat.push({ role: "assistant", content: response.data });
  
      if(!this.chatId && response.data.chatId) {
        this.chatId = response.data.chatId;
      }
  
      this.updateChatView();
    } catch (error) {
      console.log(error);
      alert('Error sending message');
    }
  }

  updateChatView(): void {
    setTimeout(() => {
      const chatContainer = document.querySelector('#chat-messages');
      if (chatContainer) {
        chatContainer.scrollTop = chatContainer.scrollHeight;
      }
    }, 100);
  }

  adjustTextareaHeight(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = Math.min(textarea.scrollHeight, 5 * 24) + 'px';
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
