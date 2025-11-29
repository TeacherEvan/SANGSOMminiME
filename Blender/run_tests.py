import unittest
import sys
import os

# Add current directory to path
sys.path.append(os.path.dirname(os.path.realpath(__file__)))

# Discover and run tests
loader = unittest.TestLoader()
start_dir = os.path.join(os.path.dirname(__file__), 'Tests')
suite = loader.discover(start_dir, pattern='test_*.py')

runner = unittest.TextTestRunner(verbosity=2)
result = runner.run(suite)

if not result.wasSuccessful():
    sys.exit(1)
