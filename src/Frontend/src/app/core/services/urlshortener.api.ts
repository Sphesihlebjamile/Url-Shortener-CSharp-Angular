import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Service, inject } from '@angular/core';

export interface ShortenUrlResponse {
  shortUrl: string;
  serverTime: number;
}

@Service()
export class UrlShortenerApi {
  private readonly http = inject(HttpClient);
  private readonly shortenUrlApiUrl = 'https://localhost:7017/api/v1/data/shorten';
  private readonly getLongUrlByShortUrlCodeApiUrl = 'https://localhost:7017/api/v1/data/long';

  shortenUrl(
    longUrl: string,
    expiryDate: string | null,
    onSuccess: (response: ShortenUrlResponse) => void,
    onError: (error: HttpErrorResponse) => void,
  ): void {
    this.http.post<ShortenUrlResponse>(this.shortenUrlApiUrl, { longUrl, expiryDate }).subscribe({
      next: onSuccess,
      error: onError,
    });
  }
}
