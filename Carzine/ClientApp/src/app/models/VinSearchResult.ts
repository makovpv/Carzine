import { CarModel } from "./CarModel";
import { CarModificationModel } from "./CarModificationModel";
import { CarPartsGroupModel } from "./CarPartsGroupModel";

export class VinSearchResult {
	type: CarTypeModel | undefined;
	groups: CarPartsGroupModel[] = [];
	modification: CarModificationModel | undefined;
	model: CarModel | undefined;
}

export class CarTypeModel {
	id: string | undefined;
}