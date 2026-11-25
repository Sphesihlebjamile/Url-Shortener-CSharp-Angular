import { Component, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UrlShortenerApi } from '../../core/services/urlshortener.api';
import { HttpErrorResponse } from '@angular/common/http';
import { ShortUrlDefault } from '../../core/constants/urlshortener.constants';

@Component({
  imports: [FormsModule],
  selector: 'app-long-url-input',
  templateUrl: './long-url-input.html',
})
export class LongUrlInput {
  private readonly urlShortenerApi = inject(UrlShortenerApi);

  private longUrl: string = '';
  private expiryDate: string = '';
  private expiryDateEnabled = false;
  readonly longUrlErrors = signal<string[]>([]);
  readonly isGenerating = signal(false);

  shortUrl = output<string>();

  handleSubmit = (): void => {
    const validationResult = this.validateLongUrl();

    if (!validationResult) {
      this.shortUrl.emit(ShortUrlDefault);
      return;
    }

    this.clearLongUrlErrors();
    this.isGenerating.set(true);

    this.urlShortenerApi.shortenUrl(
      this.longUrl,
      this.expiryDateEnabled ? this.getExpiryDateWithTimezone() : null,
      (response) => {
        this.shortUrl.emit(response.shortUrl);
        this.isGenerating.set(false);
      },
      (error: HttpErrorResponse) => {
        this.shortUrl.emit(ShortUrlDefault);
        this.isGenerating.set(false);
        this.longUrlErrors.set([this.getApiErrorMessage(error)]);
      },
    );
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
    if (this.longUrl && !this.isValidHttpUrl(this.longUrl.trim())) {
      errors = [...errors, "Enter a valid URL starting with http:// or https://"]
    }
    if (this.expiryDateEnabled && !this.expiryDate) {
      errors = [...errors, 'Expiry date and time is required'];
    } else if (this.expiryDateEnabled) {
      const expiryTime = new Date(this.expiryDate).getTime();
      if (!Number.isFinite(expiryTime) || expiryTime <= Date.now()) {
        errors = [...errors, 'Enter a valid expiry date and time in the future'];
      }
    }
    this.longUrlErrors.set(errors);
    return errors.length === 0;
  }

  private getExpiryDateWithTimezone(): string {
    const date = new Date(this.expiryDate);
    const offsetMinutes = -date.getTimezoneOffset();
    const offsetSign = offsetMinutes >= 0 ? '+' : '-';
    const absoluteOffsetMinutes = Math.abs(offsetMinutes);
    const offsetHours = String(Math.floor(absoluteOffsetMinutes / 60)).padStart(2, '0');
    const offsetRemainderMinutes = String(absoluteOffsetMinutes % 60).padStart(2, '0');
    const localDateTime = [
      `${date.getFullYear().toString().padStart(4, '0')}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`,
      `${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}:${String(date.getSeconds()).padStart(2, '0')}`,
    ].join('T');

    return `${localDateTime}${offsetSign}${offsetHours}:${offsetRemainderMinutes}`;
  }

  private isValidHttpUrl(value: string): boolean {
    try {
      const url = new URL(value);
      const domainPattern = /^(?=.{1,253}\.?$)[a-z\d](?:[a-z\d-]{0,61}[a-z\d])?(?:\.[a-z\d](?:[a-z\d-]{0,61}[a-z\d])?)+\.?$/i;
      return (url.protocol === 'http:' || url.protocol === 'https:') && domainPattern.test(url.hostname);
    } catch {
      return false;
    }
  }

  clearLongUrlErrors = (): void => {
    this.longUrlErrors.set([]);
  }

  private getApiErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'Unable to connect to the local API.';
    }

    return error.error?.title ?? error.message ?? 'Failed to generate the short URL.';
  }
}
