const moment = require('moment');
import {CommonPageHelper} from '../../common/common-page.helper';
import {HtmlHelper} from '../../../../components/misc-utils/html-helper';
import {EventsPageConstants} from './events-page.constants';
import {CommonPageConstants} from '../../common/common-page.constants';
import {By, element} from 'protractor';
import {ComponentHelpers} from '../../../../components/devfactory/component-helpers/component-helpers';

export class EventsPage {

    static get eventsTab() {
        return CommonPageHelper.getSpanByTextInsideUnorderedListByRole(HtmlHelper.tags.tabList, EventsPageConstants.events);
    }

    static get newEvent() {
        return CommonPageHelper.getElementContainsId(EventsPageConstants.newEventItem);
    }

    static get titleTextField() {
         return CommonPageHelper.getElementByTitle(EventsPageConstants.inputFields.title);
    }

    static get categoryField() {
        return CommonPageHelper.getElementByTitle(EventsPageConstants.inputFields.category);
    }

    static get categoryOption() {
        return element(By.xpath(`//option[${ComponentHelpers.getXPathFunctionForText(EventsPageConstants.categoryOption.meeting)}]`));
    }

    static get calenderBlock() {
        return CommonPageHelper.getElementContainsTitle(CommonPageConstants.title);
    }

    static get calenderTomorrow() {
        const currentDate = Date.now();
        let tomorrow = moment(currentDate).add(1, 'days');
        tomorrow = moment(tomorrow).format('M/DD/YYYY');
        return element(By.css(`td[date='${tomorrow}']`));
    }

    static get closeEventButton() {
        return element(By.css('[value = "Close"]'));
    }

    static addNewEvent(title: string) {
        return element(By.css(`a[title='${title}']`));
    }

    static getElementByLinkText(text: string) {
        return element(By.linkText(text));
    }

    static eventPageByTitle(title: string) {
        return CommonPageHelper.getElementUsingTextContent(title, true);
    }
}
