import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from '@app/app.component';
import { appConfig } from '@app/app.config';
import { registerLocaleData } from '@angular/common';
import localeUk from '@angular/common/locales/uk';

bootstrapApplication(AppComponent, appConfig).catch((err) =>
  console.error(err)
);

registerLocaleData(localeUk);
