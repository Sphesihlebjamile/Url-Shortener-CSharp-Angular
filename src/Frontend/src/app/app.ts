import { Component, signal, WritableSignal } from '@angular/core';
import { Urlshortener } from './urlshortener/urlshortener';

@Component({
  imports: [Urlshortener],
  selector: 'app-root',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('Frontend');
}
