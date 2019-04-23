import {TextBoxComponentSelectorsFactory} from '@aurea/protractor-automation-helper';
import {ModelComponentSelectors} from '../modal-component/model-component-selectors';
import {HtmlHelper} from '../../misc-utils/html-helper';
import {ComponentHelpers} from '../../devfactory/component-helpers/component-helpers';
import {By, element} from 'protractor';

export class TextComponentSelectors extends TextBoxComponentSelectorsFactory {
    public static getDivForTextXpath(
        text: string,
        isContains = false,
        insidePopup = false
    ) {
        return `${ModelComponentSelectors.getModelPopupXpath(insidePopup)}` +
            `${ComponentHelpers.getElementByTagXpath(
                HtmlHelper.tags.div,
                text,
                isContains,
            )}`;
    }

    public static getListForClassXpath(
        classValue: string,
        isContains = false,
        insidePopup = false
    ) {
        return `${ModelComponentSelectors.getModelPopupXpath(insidePopup)}` +
            `${ComponentHelpers.getElementByClassXpath(
                HtmlHelper.tags.li,
                classValue,
                isContains,
            )}`;
    }

    public static getSpanForTextXpath(
        text: string,
        isContains = false,
        insidePopup = false
    ) {
        return `${ModelComponentSelectors.getModelPopupXpath(insidePopup)}` +
            `${ComponentHelpers.getElementByTagXpath(
                HtmlHelper.tags.span,
                text,
                isContains,
            )}`;
    }

    static getSpanForTextInsideListForClassXpath(classValue: string, text: string, isContains = false) {
        return `${this.getListForClassXpath(classValue, isContains)}${this.getSpanForTextXpath(text)}`;
    }

    static getItemByText(text: string, isContains = false, tag = '*') {
        const xpath = `//${tag}[${ComponentHelpers.getXPathFunctionForText(
            text, isContains)}]`;
        return element(By.xpath(xpath));
    }

    static getListByText(text: string, textIsContains = false) {
        return this.getItemByText(text, textIsContains, HtmlHelper.tags.li);
    }

}