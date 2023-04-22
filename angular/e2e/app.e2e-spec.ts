import { Nguyen_Tan_Phat_ProjectTemplatePage } from './app.po';

describe('Nguyen_Tan_Phat_Project App', function() {
  let page: Nguyen_Tan_Phat_ProjectTemplatePage;

  beforeEach(() => {
    page = new Nguyen_Tan_Phat_ProjectTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
