import { ref, readonly, onMounted, onUnmounted } from "vue";
import { MOTION_IDS } from "~/utils/calculator.config";

export function useMotionRenderer() {
  const layers = new Map<string, SVGElement>();
  const liveMotion = new Map<string, Set<Animation>>(
    MOTION_IDS.map((id) => [id, new Set()]),
  );
  const activeMotion = ref<Set<string>>(new Set());
  const userStatic = ref(false);
  const reducedMotion = ref(false);

  const replayFades = new Set<Animation>();
  let replayToken = 0;
  let replayBusy = false;

  // --- preparation (мутация SVG до анимаций) ---
  const coldTwinkleCount = { warm: 0, cold: 0 };
  let icicleCount = 0;

  function prepareColdTwinkles(id: string, root: SVGElement): number {
    const existing = [
      ...root.querySelectorAll<SVGGElement>(".cold-twinkle-led"),
    ];
    if (!existing.length) return 0;

    const occupied = existing.map((g) => {
      const c = g.querySelector("circle")!;
      return [Number(c.getAttribute("cx")), Number(c.getAttribute("cy"))] as [
        number,
        number,
      ];
    });
    const filter =
      existing[0]?.querySelector("circle")?.getAttribute("filter") || "";
    const baseColor =
      id === "eaves-fringe-warm-twinkle" ? "#ffd39a" : "#e6f7ff";
    const baseBulbs = [
      ...root.querySelectorAll<SVGCircleElement>("[data-geometry-id] > circle"),
    ].filter(
      (c) =>
        c.getAttribute("fill") === baseColor && c.getAttribute("r") === "1.9",
    );

    const ns = "http://www.w3.org/2000/svg";
    let added = 0;
    for (let i = 7; i < baseBulbs.length && added < 10; i += 13) {
      const b = baseBulbs[i];
      const x = Number(b?.getAttribute("cx"));
      const y = Number(b?.getAttribute("cy"));
      if (occupied.some(([ox, oy]) => Math.hypot(x - ox, y - oy) < 30))
        continue;

      const g = document.createElementNS(ns, "g");
      g.setAttribute("class", "cold-twinkle-led prototype-cold-led");
      g.setAttribute("opacity", ".14");
      const specs: [number, string, string, boolean][] = [
        [17, "#a7d5ff", ".72", true],
        [4.7, "#d7f1ff", "1", false],
        [1.8, "#ffffff", "1", false],
      ];
      for (const [r, fill, op, glow] of specs) {
        const dot = document.createElementNS(ns, "circle");
        dot.setAttribute("cx", String(x));
        dot.setAttribute("cy", String(y));
        dot.setAttribute("r", String(r));
        dot.setAttribute("fill", fill);
        dot.setAttribute("opacity", op);
        if (glow && filter) dot.setAttribute("filter", filter);
        g.append(dot);
      }
      b?.parentElement?.append(g);
      occupied.push([x, y]);
      added++;
    }

    for (const g of existing) {
      const [glow, core, center] = g.querySelectorAll("circle");
      glow?.setAttribute("r", "17");
      glow?.setAttribute("fill", "#a7d5ff");
      glow?.setAttribute("opacity", ".72");
      core?.setAttribute("r", "4.7");
      core?.setAttribute("fill", "#d7f1ff");
      center?.setAttribute("r", "1.8");
    }
    return root.querySelectorAll(".cold-twinkle-led").length;
  }

  function prepareIcicleChases(id: string, root: SVGElement): number {
    if (id !== "melting-icicles") return 0;
    const filterId = root.querySelector('filter[id$="-bulbGlow"]')?.id;

    for (const core of [
      ...root.querySelectorAll<SVGCircleElement>(
        'circle[r="1.6"][opacity=".15"]',
      ),
    ]) {
      const next = core.nextElementSibling;
      const end = Number(next?.getAttribute("cy")) - 3;
      if (!Number.isFinite(end)) continue;

      core.classList.add("icicle-pulse");
      core.dataset.endY = String(end);
      core.setAttribute("r", "3.2");
      core.setAttribute("opacity", "0");

      const glow = core.cloneNode(false) as SVGCircleElement;
      glow.classList.remove("icicle-pulse");
      glow.classList.add("icicle-pulse-glow");
      glow.setAttribute("r", "8");
      if (filterId) glow.setAttribute("filter", `url(#${filterId})`);

      core.parentElement?.insertBefore(glow, core);
    }
    return root.querySelectorAll(".icicle-pulse").length;
  }

  function prepareAllLayers() {
    for (const [id, root] of layers) {
      if (id === "eaves-fringe-warm-twinkle") {
        coldTwinkleCount.warm = prepareColdTwinkles(id, root);
      } else if (id === "eaves-fringe-cold-twinkle") {
        coldTwinkleCount.cold = prepareColdTwinkles(id, root);
      } else if (id === "melting-icicles") {
        icicleCount = prepareIcicleChases(id, root);
      }
    }
  }

  // --- animation helpers ---
  const staticMotion = () => userStatic.value || reducedMotion.value;

  function track(id: string, anim: Animation): Animation {
    liveMotion.get(id)!.add(anim);
    return anim;
  }

  function cancelReplay() {
    replayToken++;
    replayBusy = false;
    for (const f of replayFades) f.cancel();
    replayFades.clear();
  }

  function stopMotion(id: string) {
    for (const a of liveMotion.get(id)!) a.cancel();
    liveMotion.get(id)!.clear();
    const svg = layers.get(id);
    if (svg) {
      svg.classList.remove("visible");
      svg.setAttribute("hidden", "");
    }
    activeMotion.value.delete(id);
    activeMotion.value = new Set(activeMotion.value);
  }

  function twinkleMotion(id: string) {
    const root = layers.get(id);
    if (!root) return;
    [...root.querySelectorAll<SVGGElement>(".cold-twinkle-led")].forEach(
      (led, i) => {
        const durations = [2600, 3300, 2900, 3700, 3100, 3500, 2800];
        const duration = durations[i % 7];
        const phase = -((i * 811 + 380) % duration!);
        track(
          id,
          led.animate(
            [
              { opacity: 0.14, offset: 0 },
              { opacity: 0.14, offset: 0.24 },
              { opacity: 1, offset: 0.34 },
              { opacity: 1, offset: 0.54 },
              { opacity: 0.14, offset: 0.65 },
              { opacity: 0.14, offset: 1 },
            ],
            { duration, delay: phase, iterations: Infinity, easing: "linear" },
          ),
        );
      },
    );
  }

  function verticalMotion(id: string, entranceDelay: number) {
    const root = layers.get(id);
    if (!root) return;
    for (const column of root.querySelectorAll<SVGGElement>(
      "[data-geometry-id]",
    )) {
      const wire = column.querySelector("path");
      if (!wire) continue;
      const wireOpacity = Number(wire.getAttribute("opacity") || 1);
      track(
        id,
        wire.animate([{ opacity: 0 }, { opacity: wireOpacity }], {
          duration: 260,
          delay: entranceDelay,
          easing: "ease-out",
          fill: "both",
        }),
      );
      const bulbs = [...column.querySelectorAll<SVGCircleElement>("circle")];
      const ys = bulbs.map((b) => Number(b.getAttribute("cy")));
      const minY = Math.min(...ys);
      const maxY = Math.max(...ys);
      bulbs.forEach((b) => {
        const y = Number(b.getAttribute("cy"));
        const target = Number(b.getAttribute("opacity") || 1);
        const delay =
          entranceDelay +
          90 +
          Math.round(((y - minY) / Math.max(1, maxY - minY)) * 530);
        track(
          id,
          b.animate([{ opacity: 0 }, { opacity: target }], {
            duration: 130,
            delay,
            easing: "ease-out",
            fill: "both",
          }),
        );
      });
    }
  }

  function beltMotion(id: string, entranceDelay: number) {
    const root = layers.get(id);
    if (!root) return;
    const bulbs = [
      ...root.querySelectorAll<SVGCircleElement>("[data-geometry-id] circle"),
    ];
    const positions = [
      ...new Set(
        bulbs.map((b) => `${b.getAttribute("cx")}:${b.getAttribute("cy")}`),
      ),
    ];
    const step = Math.min(22, 1000 / Math.max(1, positions.length - 1));
    bulbs.forEach((b) => {
      const key = `${b.getAttribute("cx")}:${b.getAttribute("cy")}`;
      const delay = entranceDelay + Math.round(positions.indexOf(key) * step);
      track(
        id,
        b.animate(
          [{ opacity: 0 }, { opacity: Number(b.getAttribute("opacity") || 1) }],
          {
            duration: 130,
            delay,
            easing: "ease-out",
            fill: "both",
          },
        ),
      );
    });
  }

  function icicleMotion(id: string) {
    const root = layers.get(id);
    if (!root) return;

    [...root.querySelectorAll<SVGCircleElement>(".icicle-pulse")].forEach(
      (core, i) => {
        const start = Number(core.getAttribute("cy"));
        const end = Number(core.dataset.endY);
        if (!Number.isFinite(start) || !Number.isFinite(end)) return;

        const dy = end - start;
        const duration = 2200 + (i % 5) * 310;
        const phase = -((i * 719 + 240) % duration);

        // glow вставлен перед core в prepareIcicleChases
        const glow = core.previousElementSibling as SVGElement | null;

        const pairs: [SVGElement | null, number][] = [
          [core, 1],
          [glow, 0.55],
        ];

        for (const [node, peak] of pairs) {
          if (!node) continue;
          track(
            id,
            node.animate(
              [
                { opacity: 0, transform: "translateY(0px)", offset: 0 },
                {
                  opacity: peak,
                  transform: `translateY(${dy * 0.55}px)`,
                  offset: 0.55,
                },
                { opacity: 0, transform: `translateY(${dy}px)`, offset: 1 },
              ],
              {
                duration,
                delay: phase,
                iterations: Infinity,
                easing: "linear",
              },
            ),
          );
        }
      },
    );
  }

  function startMotion(id: string, delay = 0, entranceDuration = 420) {
    stopMotion(id);
    const svg = layers.get(id);
    if (!svg) return;

    svg.removeAttribute("hidden");
    svg.classList.add("visible");
    activeMotion.value.add(id);
    activeMotion.value = new Set(activeMotion.value);

    if (staticMotion()) return;

    if (id === "vertical-thread") verticalMotion(id, delay);
    else if (id === "belt-light") beltMotion(id, delay);
    else
      track(
        id,
        svg.animate([{ opacity: 0 }, { opacity: 1 }], {
          duration: entranceDuration,
          delay,
          easing: "ease-in-out",
          fill: "both",
        }),
      );

    if (
      id === "eaves-fringe-warm-twinkle" ||
      id === "eaves-fringe-cold-twinkle"
    )
      twinkleMotion(id);
    if (id === "melting-icicles") icicleMotion(id);
  }

  function sync(id: string, shouldShow: boolean) {
    if (shouldShow && !activeMotion.value.has(id)) startMotion(id);
    else if (!shouldShow && activeMotion.value.has(id)) stopMotion(id);
  }

  function restartVisibleMotion() {
    cancelReplay();
    const ids = [...activeMotion.value];
    for (const id of ids) startMotion(id);
  }

  async function replayVisibleMotion(): Promise<boolean> {
    if (staticMotion() || replayBusy || !activeMotion.value.size) return false;
    cancelReplay();
    const token = replayToken;
    const ids = [...activeMotion.value];
    replayBusy = true;

    const fades = ids.map((id) => {
      const svg = layers.get(id);
      if (!svg) return Promise.resolve();
      const fade = svg.animate([{ opacity: 1 }, { opacity: 0 }], {
        duration: 500,
        easing: "ease-in-out",
        fill: "forwards",
      });
      replayFades.add(fade);
      return fade.finished.catch(() => {});
    });
    await Promise.all(fades);

    if (token !== replayToken || staticMotion()) return false;

    for (const f of replayFades) f.cancel();
    replayFades.clear();

    for (const id of ids) {
      if (!activeMotion.value.has(id)) continue;
      let delay = 0;
      if (
        (
          [
            "eaves-fringe-warm",
            "eaves-fringe-cold",
            "eaves-fringe-warm-twinkle",
            "eaves-fringe-cold-twinkle",
            "melting-icicles",
          ] as string[]
        ).includes(id)
      )
        delay = 250;
      else if (id === "vertical-thread") delay = 500;
      else if (id.startsWith("motifs-")) delay = 750;
      startMotion(id, delay, 720);
    }
    replayBusy = false;
    return true;
  }

  function toggleStatic() {
    if (reducedMotion.value) return;
    userStatic.value = !userStatic.value;
    restartVisibleMotion();
  }

  function registerLayer(id: string, el: SVGElement | null) {
    if (!el) layers.delete(id);
    else layers.set(id, el);
  }

  let mq: MediaQueryList | null = null;
  const onMqChange = (e: MediaQueryListEvent) => {
    reducedMotion.value = e.matches;
    restartVisibleMotion();
  };

  onMounted(() => {
    mq = window.matchMedia("(prefers-reduced-motion: reduce)");
    reducedMotion.value = mq.matches;
    mq.addEventListener("change", onMqChange);
  });

  onUnmounted(() => {
    mq?.removeEventListener("change", onMqChange);
    cancelReplay();
    for (const set of liveMotion.values()) for (const a of set) a.cancel();
  });

  return {
    activeMotion: readonly(activeMotion),
    userStatic,
    reducedMotion,
    coldTwinkleCount,
    get icicleCount() {
      return icicleCount;
    },
    registerLayer,
    prepareAllLayers,
    sync,
    startMotion,
    stopMotion,
    restartVisibleMotion,
    replayVisibleMotion,
    toggleStatic,
  };
}
