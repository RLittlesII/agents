Feature: Skill boundary
  A skill states a rule any role can follow and never names a role.

  Background:
    Given the synthetic checkout with roles alpha, beta and log-reader
    And log-reader signs as Zed

  @B-001
  Scenario Outline: A skill names a role
    Given a skill whose sixth line reads <line>
    When the checkout is projected
    Then projection fails naming the skill file, line 6 and <named>
    And no output directory exists

    Examples:
      | line                              | named               |
      | Ask `alpha` first.                | alpha               |
      | Hand it to the log-reader.        | log-reader          |
      | Zed signs the report.             | log-reader          |
      | See [alpha](../agents/alpha.md).  | a path into agents/ |
      | Read agents/alpha.md first.       | a path into agents/ |

  @B-001
  Scenario: Ordinary words are not role names
    Given a skill mentioning the alpha release, a log-reader-like tool, a host's agents directory and the word zed
    When the skills are linted
    Then the lint passes

  @B-002
  Scenario: A generated block names roles
    Given a skill whose generated block names alpha, log-reader and Zed
    And whose line after the block names beta
    When the checkout is projected
    Then projection fails naming beta at that line
    And names nothing inside the block

  @B-003
  Scenario: This repository's skills name no role
    Given the skills of this repository
    When the skills are linted
    Then the lint passes

  @B-001
  Scenario: An unnamed persona adds no name pattern
    Given log-reader declares only the voice Terse without a persona name
    And a skill mentions Zed in ordinary prose
    When the checkout is projected
    Then the lint passes
    But a skill naming log-reader still fails naming the role

  @B-004
  Scenario: Tracker records may name roles without becoming method instructions
    Given role-bearing tracker entries at the skill root and nested resource depths
    When skill source text is linted
    Then tracker entries are ignored
    But a role name in an ordinary runtime resource still fails with its path and line
