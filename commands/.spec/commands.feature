Feature: Commands
  A command is written once, convenes roles and names skills, and lands as a
  Claude command file and a Codex skill.

  Background:
    Given the synthetic checkout with roles alpha and beta and the skill sdd-conventions
    And both hosts declare where commands land
    And a command convene naming alpha, beta and sdd-conventions

  @B-001
  Scenario: A command projects to a Claude command file
    When the checkout is projected
    Then the Claude command file for convene carries the description and the argument hint
    And it names alpha's and beta's Claude agent files in order
    And it names the skill at its Claude path
    And the command body follows unchanged, request placeholder included

  @B-002
  Scenario: A command projects to a Codex skill
    Given a command solo naming only beta and no skill
    When the checkout is projected
    Then the Codex skill for solo carries the name and the description
    And it tells the reader to act as beta, whose contract is beta's Codex agent file
    And its request placeholder reads as the request that invoked the skill

  @B-003
  Scenario: A command names a role with no file
    Given convene names alpha and ghost
    When the checkout is projected
    Then projection fails naming the command and ghost
    And no output directory exists

  @B-004
  Scenario: A command names a skill with no directory
    Given convene names the skill ghost
    When the checkout is projected
    Then projection fails naming the command and ghost
    And no output directory exists

  @B-005
  Scenario Outline: A malformed command fails
    Given convene <defect>
    When the checkout is projected
    Then projection fails naming the command
    And no output directory exists

    Examples:
      | defect                                    |
      | declares the name other                   |
      | has a description of two lines            |
      | names no role                             |
      | links agents/alpha.md from its body       |

  @B-006
  Scenario: A host without a commands output gets no command
    Given neither host declares where commands land
    When the checkout is projected
    Then no command file and no command skill exists

  @B-006
  Scenario: A command output with no source is stale
    Given the checkout was projected
    And a command file with no source sits in the Claude commands directory
    When the checkout is checked
    Then the check names that file as stale

  @B-007
  Scenario: A command named like a skill fails
    Given a command named sdd-conventions
    When the checkout is projected
    Then projection fails naming the command and the skill
    And no output directory exists
