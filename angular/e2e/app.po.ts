import { browser, element, by } from 'protractor';

export class Nguyen_Tan_Phat_ProjectTemplatePage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('app-root h1')).getText();
  }
}
