Feature: Repository verification
  A maintainer can verify trustworthy changed-line coverage locally and in CI.

  @B-001
  Scenario Outline: Enforce the pooled changed-line floor
    Given valid current evidence for <covered> covered lines out of <changed> changed executable production lines
    When repository verification evaluates the evidence
    Then it exits <exit_code>
    And it reports the exact covered and changed line counts

    Examples:
      | covered | changed | exit_code |
      | 4 | 5 | 0 |
      | 799 | 1000 | 1 |

  @B-002
  Scenario Outline: Unusable evidence cannot pass
    Given evidence that is <problem>
    When repository verification checks the measured source snapshot
    Then it exits nonzero without a passing coverage result

    Examples:
      | problem |
      | missing |
      | malformed |
      | stale after source bytes change |
      | mapped outside the measured checkout |
      | mismapped by source separators other than LF or CRLF |
      | missing a newly added production file |
      | produced by a failed collector or test command |

  @B-003
  Scenario: No changed executable production lines
    Given successful collection with complete valid evidence
    And no changed executable production lines
    When repository verification evaluates the evidence
    Then it exits zero
    And it reports not applicable with a zero denominator and no percentage

  @B-004
  Scenario: Local and CI verification share one contract
    Given the same policy, source snapshot and comparison revision
    When local and CI contexts invoke repository verification
    Then both use the same evaluator and policy
    And executable gate tooling is included in production scope
    And both report the comparison revision and covered and changed line counts
