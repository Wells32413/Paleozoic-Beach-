# Development roadmap

## Milestone 1: vertical slice (this repository)

Deliver code for the menu/facility, controllable 3D dive, a single scientifically scoped environment, 480-second return, prototype wildlife, research photographs, results, persistence and a simplified functioning park economy. Acceptance is a complete Editor playthrough followed by a restart that retains funding and discoveries, plus passing EditMode tests. Source implementation alone is not acceptance.

## Milestone 1 validation gate

Import with supported Unity 6.3 LTS patch, resolve any API/package issues, run tests and the checklist in VERIFICATION.md. Record Editor version, target, test XML and logs. Build to one physical iPhone. Profile frame time, memory, thermals, safe areas and multitouch. Then test a representative Android device. Fix before adding content.

## Milestone 2: grounded exploration

Select a precise Illinois site/horizon with a paleontologist. Introduce validated ScriptableObject content authoring, regional/stratigraphic eligibility, bounded habitat zones, rigged Bandringa, ecological behavior states and annotated sources. Add camera image storage, video, identification, behavioral observation, scanning and repeat-research thresholds. Add objective and equipment planning without unlocking unsupported animals. Add resource hazards and meaningful navigation. Split the prototype Unity adapter into UI, persistence, submarine and wildlife components once interactions stabilize.

## Milestone 3: park simulation

Add a 3D park grid, paths, buildings, exhibit placement/upgrades, visitor agents and access validation. Then admission pricing, food/retail, staff, satisfaction, maintenance, research partnerships and balancing. Exhibits remain tied to research. Add a financially safe recovery route if players spend all funds before securing exhibit income; prototype economy can currently be exhausted through unproductive paid dives.

## Milestone 4: progression and production presentation

Introduce reviewed time/locality content, costed equipment, upgrades, additional objectives and research opportunities. Asset pipeline: scientifically reviewed models, LODs, texture budgets, animation, underwater scattering/turbidity and ambient audio. Select URP after measuring a mobile render budget. Add low/medium/high settings and device profiling; target 30 fps minimum on a defined baseline device, measured rather than assumed.

## Milestone 5: release readiness

Save migration and interruption recovery, robust accessibility, localization, input rebinding, safe-area layouts, platform permissions, crash reporting/privacy choices, distribution signing and store compliance. No mandatory advertising, pay-to-win or artificial waiting periods. CI should gate Core tests, Unity import/compile, play-mode smoke tests and platform builds using appropriately licensed runners. Each accepted milestone gets a reviewed GitHub commit/tag with evidence; preserve the preceding working build.
