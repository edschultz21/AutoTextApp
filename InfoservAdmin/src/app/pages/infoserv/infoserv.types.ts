
export interface Environment {
  name: string;
  url: string;
}

export interface DataType {
  Type: string;
}

export interface DataResult extends DataType {
  Payload: DataType;
}

export interface DataSegmentResult extends DataType {
  Id: string[];
  Key: string;
  Segment: Segment;
  Position: SegmentPosition;
}

export interface DataDataResult extends DataType {
  MetricId: string;
  SegmentKey: string;
  SegmentId: string[];
  Data: DataDataPoint[];
}

export interface DataDataPoint {
  Value: number;
  Period: string;
  PeriodDate: Date;
  AdditionalInfo: object;
}

export interface DataDebugResult extends DataType {
  Debug: string;
}

export interface DataError extends DataType {
  ErrorMessage: string;
}

export interface DetailsResult {
  DisplayId: string;
  MLSOfficeKey: string;
  MLSOfficeId: string;
  Name: string;
  Website: string;
  OfficePhone: string;
  Email: string;
  ObjectType: string;
  Segments: Segment[];
}

export interface Segment {
  ObjectType: string;
  SegmentKey: string;
  DisplayName: string;
  IsSearchable: string;
  SegmentGroupKey: string;
  SegmentType: string;
}

export interface SegmentPosition {
  Value: number;
  Total: number;
}

export interface SegmentSearchResult {
  ObjectType: string;
  SegmentKey: string;
  DisplayName: string;
  IsSearchable: boolean;
  SegmentGroupKey: string;
  SegmentType: string;
}

export enum DisplayResultEnum {
  Default,
  Details,
  Data,
  Debug,
  SegmentSearch
}
