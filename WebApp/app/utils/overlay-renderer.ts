import type {
  Geometry,
  EavesPath,
  RoofPath,
  VerticalPath,
  SnowflakeAnchor,
  PolarisAnchor,
  KeepoutZone,
  Point,
} from "~/types/geometry";

type OverlayOptions = {
  polarisImageUrl?: string;
  anchorIndex?: number;
  layerId?: string;
};

const pickAnchor = <T>(items: T[], index?: number): T[] =>
  index === undefined ? items : items.slice(index, index + 1);

const WARM = "#ffd39a";
const COLD = "#e6f7ff";
const COLD_TWINKLE = "#f7fdff";

const fmt = (v: number) => Number(v.toFixed(2));
const distance = (a: Point, b: Point) => Math.hypot(b[0] - a[0], b[1] - a[1]);

export const pathData = (points: Point[]) =>
  `M ${points.map(([x, y]) => `${fmt(x)} ${fmt(y)}`).join(" L ")}`;

export function samplePath(points: Point[], step = 28): Point[] {
  const result: Point[] = [];
  for (let i = 0; i < points.length - 1; i++) {
    const start = points[i];
    const end = points[i + 1];
    const count = Math.max(1, Math.ceil(distance(start!, end!) / step));
    for (let j = 0; j < count; j++) {
      const t = j / count;
      const p: Point = [
        start![0] + (end![0] - start![0]) * t,
        start![1] + (end![1] - start![1]) * t,
      ];
      const prev = result[result.length - 1];
      if (!prev || distance(prev, p) > 0.1) result.push(p);
    }
  }

  const last = points.at(-1);
  if (last) result.push(last);

  return result;
}

function snowflakeSymbol(): string {
  const arm =
    '<path d="M0 0V-34 M0-13L-7-20 M0-13L7-20 M0-23L-6-29 M0-23L6-29"/>';
  const arms = [0, 60, 120, 180, 240, 300]
    .map((a) => `<g transform="rotate(${a})">${arm}</g>`)
    .join("");
  return `<g id="ksSnowflake" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round">${arms}<circle cx="0" cy="0" r="3.2"/></g>`;
}

function polarisSymbol(imageUrl: string): string {
  return `<symbol id="ksPolaris" viewBox="0 0 1024 1536"><image x="0" y="0" width="1024" height="1536" href="${imageUrl}" preserveAspectRatio="xMidYMid meet"/></symbol>`;
}

export function svgDefs({
  polarisImageUrl,
}: { polarisImageUrl?: string } = {}): string {
  return `<defs>
    <filter id="flexGlow" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="5"/></filter>
    <filter id="bulbGlow" x="-200%" y="-200%" width="500%" height="500%"><feGaussianBlur stdDeviation="3.2"/></filter>
    <filter id="motifGlow" x="-80%" y="-80%" width="260%" height="260%"><feGaussianBlur stdDeviation="2.8"/></filter>
    ${snowflakeSymbol()}
    ${polarisImageUrl ? polarisSymbol(polarisImageUrl) : ""}
  </defs>`;
}

function flex(paths: RoofPath[], color: string, geometry: Geometry): string {
  const d0 = geometry.layerDefaults?.roofFlex || {
    coreWidth: 2.4,
    glowWidth: 10,
  };
  return paths
    .map((path) => {
      const d = pathData(path.points);
      return `<g data-geometry-id="${path.id}">
      <path d="${d}" fill="none" stroke="${color}" stroke-width="${d0.glowWidth}" stroke-linecap="round" stroke-linejoin="round" opacity=".27" filter="url(#flexGlow)"/>
      <path d="${d}" fill="none" stroke="${color}" stroke-width="${d0.coreWidth}" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="${d}" fill="none" stroke="#ffffff" stroke-width=".7" stroke-linecap="round" stroke-linejoin="round" opacity=".62"/>
    </g>`;
    })
    .join("");
}

function safeDropLength(
  geometry: Geometry,
  x: number,
  y: number,
  requested: number,
): number {
  let endY = y + requested;
  for (const zone of geometry.keepoutZones || []) {
    const insideX = x >= zone.x - 3 && x <= zone.x + zone.width + 3;
    const intersectsY = endY > zone.y - 6 && y < zone.y + zone.height;
    if (!insideX || !intersectsY) continue;
    if (y >= zone.y - 9) return 0;
    endY = Math.min(endY, zone.y - 8);
  }
  return endY - y >= 14 ? endY - y : 0;
}

function bulb(
  x: number,
  y: number,
  color: string,
  radius: number,
  opacity = 1,
): string {
  return `<circle cx="${fmt(x)}" cy="${fmt(y)}" r="${fmt(radius * 2.4)}" fill="${color}" opacity="${fmt(0.2 * opacity)}" filter="url(#bulbGlow)"/><circle cx="${fmt(x)}" cy="${fmt(y)}" r="${fmt(radius)}" fill="${color}" opacity="${opacity}"/>`;
}

function twinkle(x: number, y: number): string {
  return `<g class="cold-twinkle-led" opacity=".45">${bulb(x, y, COLD_TWINKLE, 2.25)}<circle cx="${fmt(x)}" cy="${fmt(y)}" r=".8" fill="#ffffff"/></g>`;
}

function fringe(
  paths: EavesPath[],
  geometry: Geometry,
  color: string,
  { withTwinkle = false } = {},
): string {
  const d0 = geometry.layerDefaults?.fringe || {
    spacing: 25,
    wireWidth: 1.05,
    bulbRadius: 1.9,
  };
  const pattern = [24, 38, 31, 52, 28, 44, 62, 35, 48];
  let markup = "";
  for (const path of paths) {
    const scale = path.dropScale || 1;
    const d = pathData(path.points);
    markup += `<g data-geometry-id="${path.id}"><path d="${d}" fill="none" stroke="#182431" stroke-width="2" stroke-linecap="round"/><path d="${d}" fill="none" stroke="${color}" stroke-width=".75" stroke-linecap="round" opacity=".72"/>`;
    samplePath(path.points, d0.spacing).forEach(([x, y], index) => {
      const rawPattern = pattern[index % pattern.length];
      if (rawPattern === undefined) return;

      const raw = rawPattern * scale;
      const length = safeDropLength(geometry, x, y, raw);
      if (!length) return;
      const endX = x + ((index % 3) - 1) * 0.65;
      const endY = y + length;
      markup += `<path d="M${fmt(x)} ${fmt(y)}L${fmt(endX)} ${fmt(endY)}" fill="none" stroke="#182431" stroke-width="${d0.wireWidth + 0.7}" stroke-linecap="round"/><path d="M${fmt(x)} ${fmt(y)}L${fmt(endX)} ${fmt(endY)}" fill="none" stroke="${color}" stroke-width="${d0.wireWidth}" stroke-linecap="round" opacity=".72"/>`;
      for (let off = 9; off < length; off += 10) {
        const t = off / length;
        markup += bulb(x + (endX - x) * t, y + off, color, d0.bulbRadius, 0.95);
      }
      markup += bulb(endX, endY, color, d0.bulbRadius * 1.08);
      if (withTwinkle && index % 11 === 3)
        markup += twinkle(endX, endY - Math.min(10, length * 0.35));
    });
    markup += "</g>";
  }
  return markup;
}

function meltingIcicles(paths: EavesPath[], geometry: Geometry): string {
  const d0 = geometry.layerDefaults?.icicles || {
    spacing: 48,
    wireWidth: 1.05,
    bulbRadius: 1.75,
  };
  const spacing = Math.max(44, d0.spacing);
  const lengths = [66, 105, 82, 126, 74, 112, 94];
  let markup = "";
  for (const path of paths) {
    const d = pathData(path.points);
    markup += `<g data-geometry-id="${path.id}"><path d="${d}" fill="none" stroke="#1a2631" stroke-width="1.2" opacity=".8"/>`;

    samplePath(path.points, spacing).forEach(([x, y], index) => {
      const baseLen = lengths[index % lengths.length];
      if (baseLen === undefined) return;

      const length = safeDropLength(
        geometry,
        x,
        y,
        baseLen * (path.dropScale || 1),
      );
      if (length < 23) return;
      const endY = y + length;
      markup += `<path d="M${fmt(x)} ${fmt(y)}V${fmt(endY)}" fill="none" stroke="#d9f2ff" stroke-width="5.2" stroke-linecap="round" opacity=".14" filter="url(#bulbGlow)"/>`;
      markup += `<path d="M${fmt(x)} ${fmt(y)}V${fmt(endY)}" fill="none" stroke="#a8c6d9" stroke-width="3.2" stroke-linecap="round" opacity=".62"/>`;
      markup += `<path d="M${fmt(x)} ${fmt(y + 3)}V${fmt(endY - 2)}" fill="none" stroke="${COLD}" stroke-width="1.15" stroke-linecap="round" opacity=".8"/>`;
      for (let off = 8; off < length - 4; off += 8) {
        markup += `<rect x="${fmt(x - 0.9)}" y="${fmt(y + off)}" width="1.8" height="3.3" rx=".7" fill="#ffffff" opacity="${index % 3 === 1 ? ".91" : ".69"}"/>`;
      }
      markup += `<circle cx="${fmt(x)}" cy="${fmt(y + 7)}" r="1.6" fill="#ffffff" opacity=".15"></circle>`;
      markup += bulb(x, endY - 2, COLD, 1.2, 0.75);
    });
    markup += "</g>";
  }
  return markup;
}

function beltLight(paths: RoofPath[], geometry: Geometry): string {
  const radius = 2.75;
  let markup = "";
  for (const path of paths) {
    const d = pathData(path.points);
    markup += `<g data-geometry-id="${path.id}"><path d="${d}" fill="none" stroke="#1a222b" stroke-width="1.5" stroke-linecap="round"/>`;
    samplePath(path.points, 42).forEach(([x, y]) => {
      markup += bulb(x, y, WARM, radius);
    });
    markup += "</g>";
  }
  return markup;
}

function verticalThread(paths: VerticalPath[], geometry: Geometry): string {
  const d0 = geometry.layerDefaults?.verticalThread || {
    wireWidth: 1.1,
    bulbRadius: 2.2,
  };
  const radius = Math.min(d0.bulbRadius, 1.65);
  const wireWidth = Math.min(d0.wireWidth, 0.7);
  let markup = "";
  for (const path of paths) {
    const points = samplePath(path.points, 9).map(
      (p, i) => [p[0] + Math.sin(i * 0.83) * 3.2, p[1]] as Point,
    );
    const d = pathData(points);
    markup += `<g data-geometry-id="${path.id}"><path d="${d}" fill="none" stroke="#4b3a29" stroke-width="${wireWidth}" stroke-linecap="round" stroke-linejoin="round" opacity=".85"/>`;
    points.forEach((p, i) => {
      if (i % 2 === 0) markup += bulb(p[0], p[1], WARM, radius, 0.92);
    });
    markup += "</g>";
  }
  return markup;
}

function snowflakes(anchors: SnowflakeAnchor[], geometry: Geometry): string {
  const width = geometry.layerDefaults?.motif?.strokeWidth || 2.1;
  return anchors
    .map(
      (a) =>
        `<g data-anchor-id="${a.id}" transform="translate(${a.point[0]} ${a.point[1]}) scale(${a.scale || 1})" color="${COLD}" stroke-width="${width}"><use href="#ksSnowflake" opacity=".3" filter="url(#motifGlow)"/><use href="#ksSnowflake"/></g>`,
    )
    .join("");
}

function polaris(
  anchors: PolarisAnchor[],
  geometry: Geometry,
  { polarisImageUrl }: { polarisImageUrl?: string } = {},
): string {
  if (!polarisImageUrl) throw new Error("Polaris image data is missing.");
  return anchors
    .map((a) => {
      const [mx, my] = a.mountPoint;
      const [x, y] = a.point;
      const isPorch = a.id.startsWith("polaris-porch-");
      const scale = a.scale || 1;
      const width = (isPorch ? 50 : 108) * scale;
      const height = (isPorch ? 75 : 162) * scale;
      const topY = y - 22 * scale;
      const wireY = topY + height * 0.045;
      return `<g data-anchor-id="${a.id}" data-render="3d-acrylic-polaris"><path d="M${fmt(mx)} ${fmt(my)}L${fmt(x)} ${fmt(wireY)}" fill="none" stroke="#aebbc4" stroke-width="1.15" opacity=".88"/><use href="#ksPolaris" x="${fmt(x - width / 2)}" y="${fmt(topY)}" width="${fmt(width)}" height="${fmt(height)}"/></g>`;
    })
    .join("");
}

export function buildOverlayBody(
  id: string,
  geometry: Geometry,
  options: OverlayOptions = {},
): string {
  switch (id) {
    case "roof-flex-warm":
      return flex(geometry.roofPaths, WARM, geometry);
    case "roof-flex-cold":
      return flex(geometry.roofPaths, COLD, geometry);
    case "belt-light":
      return beltLight(geometry.roofPaths, geometry);
    case "eaves-fringe-warm":
      return fringe(geometry.eavesPaths, geometry, WARM);
    case "eaves-fringe-cold":
      return fringe(geometry.eavesPaths, geometry, COLD);
    case "eaves-fringe-warm-twinkle":
      return fringe(geometry.eavesPaths, geometry, WARM, { withTwinkle: true });
    case "eaves-fringe-cold-twinkle":
      return fringe(geometry.eavesPaths, geometry, COLD, { withTwinkle: true });
    case "melting-icicles":
      return meltingIcicles(geometry.eavesPaths, geometry);
    case "vertical-thread":
      return verticalThread(geometry.verticalPaths, geometry);
    case "motifs-snowflakes":
      return snowflakes(
        pickAnchor(geometry.snowflakeAnchors, options.anchorIndex),
        geometry,
      );
    case "motifs-polaris":
      return polaris(
        pickAnchor(geometry.polarisAnchors, options.anchorIndex),
        geometry,
        options,
      );
    default:
      throw new Error(`Unknown overlay: ${id}`);
  }
}

export function buildOverlaySvg(
  id: string,
  geometry: Geometry,
  options: OverlayOptions = {},
): string {
  const { width, height } = geometry.canvas;
  return `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}" preserveAspectRatio="none" data-overlay-id="${options.layerId ?? id}">${svgDefs(id === "motifs-polaris" ? options : {})}<g>${buildOverlayBody(id, geometry, options)}</g></svg>`;
}
