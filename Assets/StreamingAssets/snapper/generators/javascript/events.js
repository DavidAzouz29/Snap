goog.require('Blockly.JavaScript');

Blockly.JavaScript['events_start'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = 'void Start()\n{\n' + statements_code + '}';
  return code;
};

Blockly.JavaScript['events_update'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = 'void Update()\n{\n' + statements_code + '}';
  return code;
};

Blockly.JavaScript['events_awake'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = 'void Awake()\n{\n' + statements_code + '}';
  return code;
};

Blockly.JavaScript['events_init'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // TODO: Assemble JavaScript into code variable.
  var code = statements_code + '\n';
  return code;
};
