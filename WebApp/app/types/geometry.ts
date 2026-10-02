export type Point = [number, number];

export interface RoofPath {
  id: string;
  label: string;
  zone: string;
  points: Point[];
}

export interface EavesPath extends RoofPath {
  dropScale: number;
}

export interface VerticalPath {
  id: string;
  label: string;
  zone: string;
  points: Point[];
}

export interface SnowflakeAnchor {
  id: string;
  label: string;
  point: Point;
  scale: number;
  zone: string;
}

export interface PolarisAnchor {
  id: string;
  label: string;
  point: Point;
  mountPoint: Point;
  scale: number;
  placement: string;
  zone: string;
  attachedTo: string;
}

export interface KeepoutZone {
  id: string;
  label: string;
  x: number;
  y: number;
  width: number;
  height: number;
}

export interface LayerDefaults {
  roofFlex?: { coreWidth: number; glowWidth: number };
  fringe?: { spacing: number; wireWidth: number; bulbRadius: number };
  icicles?: { spacing: number; wireWidth: number; bulbRadius: number };
  verticalThread?: { wireWidth: number; bulbRadius: number };
  motif?: { strokeWidth: number };
}

export interface Geometry {
  schema: string;
  version: string;
  status: string;
  canvas: {
    width: number;
    height: number;
    coordinateSystem: string;
    origin: string;
    base: string;
  };
  roofPaths: RoofPath[];
  eavesPaths: EavesPath[];
  verticalPaths: VerticalPath[];
  snowflakeAnchors: SnowflakeAnchor[];
  polarisAnchors: PolarisAnchor[];
  keepoutZones: KeepoutZone[];
  layerDefaults: LayerDefaults;
}
