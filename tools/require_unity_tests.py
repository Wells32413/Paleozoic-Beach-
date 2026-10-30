"""Fail a build if Unity did not report a nonempty, successful test run."""
import sys
import xml.etree.ElementTree as ET


def validate(path):
    root = ET.parse(path).getroot()
    if root.tag != 'test-run':
        raise ValueError('Expected NUnit test-run XML')
    total = int(root.get('total', '0'))
    passed = int(root.get('passed', '0'))
    if root.get('result') != 'Passed' or total == 0 or passed != total:
        raise ValueError(f'Unity tests did not all pass: {passed}/{total}, result={root.get("result")}')
    return total


if __name__ == '__main__':
    try:
        print(f'Unity test gate passed: {validate(sys.argv[1])} tests')
    except (OSError, ValueError, ET.ParseError, IndexError) as error:
        print(f'Unity test gate failed: {error}', file=sys.stderr)
        sys.exit(1)
