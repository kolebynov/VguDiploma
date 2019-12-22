import * as mom from "moment";
import { resources } from "./resources";
import "moment/locale/ru";

const moment = mom.default;

var locale = resources.getResourceSet("locale").getLocalizableValue("locale");
moment.locale(locale);

function toUiDateTime(dateTime: string) {
    return moment(dateTime).format("LLL");
}

export { toUiDateTime };