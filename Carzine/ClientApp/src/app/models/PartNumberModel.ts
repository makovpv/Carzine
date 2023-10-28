export class PartNumberModel {
    description: string = "";
    id: string = "";
    labelId: string = "";
    name: string = "";
    number: string = "";
}

export class PartImageLabel {
    id: string = "";
    coordinate: LabelCoordinate | undefined;
    name: string = "";
}

export class LabelCoordinate {
    bottom: CoordinatePoint | undefined;
    top: CoordinatePoint | undefined;
    height: number = 0;
    width: number = 0;
}

export class CoordinatePoint {
    x: number = 0;
    y: number = 0;
}

export class GroupPartListModel {
    numbers: PartNumberModel[] = [];
    image: string = "";
    labels: PartImageLabel[] = [];
    group: any;
}