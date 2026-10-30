import tempfile
import unittest
from pathlib import Path
from require_unity_tests import validate

class TestUnityReportGate(unittest.TestCase):
    def run_report(self, xml):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory)/'results.xml'
            path.write_text(xml)
            return validate(path)

    def test_successful_nonempty_run(self):
        self.assertEqual(8,self.run_report('<test-run result="Passed" total="8" passed="8"/>'))

    def test_failure_empty_and_skipped_reports_block_export(self):
        for xml in ['<test-run result="Failed" total="8" passed="7"/>',
                    '<test-run result="Passed" total="0" passed="0"/>',
                    '<test-run result="Passed" total="8" passed="7"/>',
                    '<unexpected/>']:
            with self.subTest(xml=xml), self.assertRaises(ValueError):
                self.run_report(xml)

if __name__ == '__main__':
    unittest.main()
