Feature: Role contract
  A role file is a host-free contract: fixed frontmatter keys, four body
  headings, and no product technology.

  Background:
    Given the role files under agents/
    And the roles index agents/README.md
    And the specification template under .specht/templates/

  @B-001
  Scenario: Every role declares the contract keys
    When the role files are loaded
    Then each declares a name equal to its file name
    And each declares a one-line description
    And each declares a semver version
    And each declares its owned specification sections
    And none carries a model, an effort, a tool list or a reasoning effort

  @B-002
  Scenario: Every template section has exactly one owner
    Given the sections each role owns
    And the sections the roles index declares as owned by nobody in particular
    When the ownership table is computed against the template
    Then every template section has exactly one owner
    And no owner names a section the template lacks

  @B-003
  Scenario: Every role body carries the four headings
    When the role files are loaded
    Then each body carries the headings Owns, Read first and Refuse
    And each body carries Produce or Look for

  @B-004
  Scenario: A role body names no product technology
    Given the names of a consuming project's language, libraries, paths and build tools
    When a reviewer searches the role bodies for them
    Then none appears
    And the companion skill of the consuming repository is where those names live

  @boundary
  Scenario: A role owns nothing
    Given a role whose owned specification sections are empty
    When the role files are loaded
    Then the role loads
    And it has no row in the ownership table
