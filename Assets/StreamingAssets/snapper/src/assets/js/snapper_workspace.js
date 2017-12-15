'use strict';
var workspace = null;
var fakeDragStack = [];

function start() {
  setBackgroundColor();

  // Parse the URL arguments.
  var match = location.search.match(/dir=([^&]+)/);
  var rtl = match && match[1] == 'rtl';
  document.forms.options.elements.dir.selectedIndex = Number(rtl);

  var toolbox = getToolboxElement();
  document.forms.options.elements.toolbox.selectedIndex =
  Number(toolbox.getElementsByTagName('category').length == 0);
  match = location.search.match(/side=([^&]+)/);
  var side = match ? match[1] : 'start';
  document.forms.options.elements.side.value = side;
  // Create main workspace.
  workspace = Blockly.inject('blocklyDiv',
  { comments: true,
    collapse: true,
    disable: true,
    grid:
    {spacing: 25,
      length: 3,
      colour: '#ccc',
      snap: true},
      horizontalLayout: side == 'top' || side == 'bottom',
      maxBlocks: Infinity,
      media: 'https://blockly-demo.appspot.com/static/media/',
      oneBasedIndex: true,
      readOnly: false,
      rtl: rtl,
      scrollbars: true,
      toolbox: toolbox,
      toolboxPosition: side == 'top' || side == 'start' ? 'start' : 'end',
      zoom:
      {controls: true,
        wheel: true,
        startScale: 1.0,
        maxScale: 4,
        minScale: .25,
        scaleSpeed: 1.1}
      });

      // Restore previously displayed text.
      if (sessionStorage) {
        var text = sessionStorage.getItem('textarea');
        if (text) {
          document.getElementById('importExport').value = text;
        }
        // Restore event logging state.
        var state = sessionStorage.getItem('liveUpdates');
        liveUpdates(Boolean(Number(state)));
        var stateLog = sessionStorage.getItem('logEvents');
        logEvents(Boolean(Number(stateLog)));
      } else {
        // MSIE 11 does not support sessionStorage on file:// URLs.
        liveUpdates(true);
        logEvents(false);
      }
      taChange();
    }

    function setBackgroundColor() {
      var lilac = '#d6d6ff';

      var currentPage = window.location.href;
      var regexFile = /^file[\S]*$/;

      if (regexFile.test(currentPage)) {
        document.getElementsByTagName('body')[0].style.backgroundColor = lilac;
      }
    }

    function getToolboxElement() {
      var match = location.search.match(/toolbox=([^&]+)/);
      return document.getElementById('toolbox-' + (match ? match[1] : 'categories')); //'snapper'));
    }

    function toXml() {
      var output = document.getElementById('importExport');
      var xml = Blockly.Xml.workspaceToDom(workspace);
      output.value = Blockly.Xml.domToPrettyText(xml);
      output.focus();
      output.select();
      taChange();
    }

    function fromXml() {
      var input = document.getElementById('importExport');
      var xml = Blockly.Xml.textToDom(input.value);
      Blockly.Xml.domToWorkspace(xml, workspace);
      taChange();
    }

    function toCode(lang, fileExtention) {
      var output = document.getElementById('importExport');
      output.value = Blockly[lang].workspaceToCode(workspace);
      taChange();
    }

    // Disable the "Import from XML" button if the XML is invalid.
    // Preserve text between page reloads.
    function taChange() {
      var textarea = document.getElementById('importExport');
      if (sessionStorage) {
        sessionStorage.setItem('textarea', textarea.value);
      }
      var valid = true;
      try {
        Blockly.Xml.textToDom(textarea.value);
      } catch (e) {
        valid = false;
      }
      document.getElementById('import').disabled = !valid;
    }

    function liveUpdates(state) {
      var checkbox = document.getElementById('liveCheck');
      checkbox.checked = state;
      if (sessionStorage) {
        sessionStorage.setItem('logEvents', Number(state));
      }
      if (state) {
        workspace.addChangeListener(loggerTextarea);
      } else {
        workspace.removeChangeListener(loggerTextarea);
      }
    }

    function logEvents(stateLog) {
      var checkbox = document.getElementById('logCheck');
      checkbox.checked = stateLog;
      if (sessionStorage) {
        sessionStorage.setItem('logEvents', Number(stateLog));
      }
      if (stateLog) {
        workspace.addChangeListener(logger);
      } else {
        workspace.removeChangeListener(logger);
      }
    }

    function loggerTextarea(e) {
      toCode('JavaScript');
    }

    function logger(e) {
      console.log(e);
    }
