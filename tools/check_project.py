"""Repository consistency only: does not compile C# or validate Unity behavior."""
import json
from pathlib import Path

root = Path(__file__).resolve().parents[1]
checks = 0

def check(condition, message):
    global checks
    assert condition, message
    checks += 1

for path in sorted(root.rglob('*.json')) + sorted(root.rglob('*.asmdef')):
    json.loads(path.read_text())
    checks += 1
catalog = json.loads((root/'Assets/PaleozoicBeach/Resources/species.json').read_text())['items']
check(len({s['id'] for s in catalog}) == len(catalog), 'duplicate species ids')
check([s['id'] for s in catalog if s['enabled']] == ['bandringa'], 'unreviewed animal enabled')
for species in catalog:
    check(species['oldestMa'] >= species['youngestMa'], 'reversed date bounds')
    check(bool(species['uncertainty']), 'missing scientific uncertainty')
core = root/'Assets/PaleozoicBeach/Core'
check(all('using UnityEngine' not in f.read_text() for f in core.glob('*.cs')), 'Core depends on Unity')
check(json.loads((core/'PaleozoicBeach.Core.asmdef').read_text())['noEngineReferences'], 'engine dependency allowed')
source = (core/'Simulation.cs').read_text()
check('const float Duration = 480' in source, 'eight minute specification changed')
check((root/'Assets/PaleozoicBeach/Tests/EditMode/SimulationTests.cs').read_text().count('[Test]') == 8, 'expected test catalog changed')
check('RuntimeInitializeOnLoadMethod' in (root/'Assets/PaleozoicBeach/Runtime/BeachGame.cs').read_text(), 'missing runtime entry')
check('6000.3.' in (root/'ProjectSettings/ProjectVersion.txt').read_text(), 'LTS pin absent')
for document in ['README.md','docs/SCIENCE.md','docs/ROADMAP.md','docs/VERIFICATION.md']:
    check((root/document).is_file(), 'missing '+document)
print(f'PASS: {checks} repository consistency checks. C# and Unity execution NOT performed.')
