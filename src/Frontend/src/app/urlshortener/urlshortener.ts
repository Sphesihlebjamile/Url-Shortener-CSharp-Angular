import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { copyToClipboard } from './urlshortener.functions';

const ShortUrlDefault: string = '...';

@Component({
  imports: [FormsModule],
  selector: 'app-urlshortener',
  styleUrl: './urlshortener.css',
  templateUrl: './urlshortener.html',
})
export class Urlshortener {
  longUrl: string = '';
  longUrlErrors: string[] = [];
  shortUrl: string = ShortUrlDefault;

  updateLongUrl = (e: Event): void => {
    console.log(e);
  }

  handleSubmit = (): void => {
    var validationResult = this.validateLongUrl();
    if(validationResult){
      this.shortUrl = this.longUrl;
      return;
    }
    this.shortUrl = ShortUrlDefault;
  }

  validateLongUrl = (): boolean => {
    let errors: string[] = [];
    if(!this.longUrl){
      errors = [...errors, "A long Url is required"]
    }
    if(this.longUrl.length < 10){
      errors = [...errors, "Long Url is too short"]
    }
    if(this.longUrl.length > 999){
      errors = [...errors, "Long Url is too long"]
    }
    this.longUrlErrors = errors;
    return errors.length === 0;
  }

  copyToClipboard = async (): Promise<void> => {
    var response = await copyToClipboard(this.shortUrl)
    if(!response){
      alert("Failed to copy to clipboard");
    }
  }
}