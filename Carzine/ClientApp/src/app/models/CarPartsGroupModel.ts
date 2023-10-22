export class CarPartsGroupModel {
	description: string | undefined;
	hasParts: boolean | undefined;
	hasSubgroups: boolean | undefined;
	id: string = "";
	image: string | undefined;
	name: string | undefined;
	needLoadSubgroups: boolean | undefined;
	parentId: string | undefined;
}