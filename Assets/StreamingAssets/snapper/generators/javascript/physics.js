goog.require('Blockly.JavaScript');

Blockly.JavaScript['physics_oncollisionenter'] = function(block) {
  var statements_code = Blockly.JavaScript.statementToCode(block, 'code');
  // Detects Collisions.
  var code = 'private void OnCollisionEnter(Collision collision)\n{\n' + statements_code + '}';
  return code;
};
