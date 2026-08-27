import AppKit
import SwiftUI

struct ChecklistView: View {
    @Bindable var store: TaskStore
    let controller: FloatingWindowController

    @State private var input = ""
    @State private var showingHistory = false
    @State private var draggedID: UUID?
    @State private var dropTargetID: UUID?
    @State private var dropAtEnd = false
    @State private var taskRowFrames: [UUID: CGRect] = [:]
    @State private var deleteCandidate: TaskItem?
    @State private var languageVersion = UUID()
    @State private var panelLockVersion = UUID()
    @AppStorage("appAppearance") private var appearanceRawValue = AppearanceMode.light.rawValue
    @FocusState private var inputFocused: Bool

    private var appearanceMode: AppearanceMode {
        AppearanceMode(rawValue: appearanceRawValue) ?? .light
    }

    var body: some View {
        ZStack {
            RoundedRectangle(cornerRadius: 14, style: .continuous)
                .fill(Color.liteTickSurface)

            VStack(spacing: 0) {
                header
                Divider().opacity(0.45)
                if showingHistory { HistoryView(store: store, deleteCandidate: $deleteCandidate) }
                else { listContent }
            }
            .id(languageVersion)
            .clipShape(RoundedRectangle(cornerRadius: 14, style: .continuous))
        }
        .preferredColorScheme(appearanceMode.colorScheme)
        .onHover(perform: controller.listHoverChanged)
        .onChange(of: input) { oldValue, newValue in
            if oldValue != newValue { controller.pinPanel() }
        }
        .onReceive(NotificationCenter.default.publisher(for: .liteTickPanelWillClose)) { _ in
            inputFocused = false
        }
        .onReceive(NotificationCenter.default.publisher(for: .liteTickPanelDidOpen)) { _ in
            inputFocused = false
        }
        .alert(L10n.deleteTitle, isPresented: Binding(
            get: { deleteCandidate != nil },
            set: { if !$0 { deleteCandidate = nil } }
        ), presenting: deleteCandidate) { item in
            Button(L10n.cancel, role: .cancel) { deleteCandidate = nil }
            Button(L10n.delete, role: .destructive) { store.delete(item.id); deleteCandidate = nil }
        } message: { _ in Text(L10n.deleteMessage) }
    }

    private var header: some View {
        HStack(spacing: 10) {
            if showingHistory {
                Button {
                    showingHistory = false
                    inputFocused = false
                } label: {
                    Image(systemName: "chevron.left")
                }
                .buttonStyle(HeaderButtonStyle())
                .fastHelp(L10n.back, horizontalOffset: 32)
            } else {
                Button {
                    controller.showAbout()
                } label: {
                    ZStack {
                        RoundedRectangle(cornerRadius: 8, style: .continuous).fill(Color.liteTickGreen)
                        Image(systemName: "checkmark").font(.system(size: 13, weight: .heavy)).foregroundStyle(.white)
                    }
                    .frame(width: 28, height: 28)
                }
                .buttonStyle(.plain)
                .fastHelp(L10n.about)
            }
            Text(showingHistory ? L10n.completed : L10n.appName)
                .font(.system(size: 15, weight: .semibold))
                .lineLimit(1)
                .fixedSize(horizontal: true, vertical: false)
            Spacer()
            Button {
                L10n.toggleLanguage()
                languageVersion = UUID()
                controller.refreshAbout()
            } label: {
                Image(systemName: "globe")
                    .font(.system(size: 13, weight: .medium))
            }
            .buttonStyle(HeaderButtonStyle())
            .fastHelp(L10n.text("Switch to English", "切换到中文"))

            Button {
                controller.transitionAppearance {
                    appearanceRawValue = appearanceMode == .light
                        ? AppearanceMode.dark.rawValue
                        : AppearanceMode.light.rawValue
                }
            } label: {
                Image(systemName: appearanceMode.symbolName)
                    .font(.system(size: 13, weight: .medium))
            }
            .buttonStyle(HeaderButtonStyle())
            .fastHelp(appearanceMode == .light ? L10n.switchToDark : L10n.switchToLight)

            if !showingHistory {
                Button {
                    showingHistory = true
                } label: {
                    Image(systemName: "checkmark.circle")
                }
                .buttonStyle(HeaderButtonStyle())
                .fastHelp(L10n.completed)
            }

            Button {
                controller.togglePanelLock()
                panelLockVersion = UUID()
            } label: {
                Image(systemName: controller.isPanelLocked ? "pin.fill" : "pin")
                    .foregroundStyle(controller.isPanelLocked ? Color.liteTickGreen : Color.primary)
                    .id(panelLockVersion)
            }
            .buttonStyle(HeaderButtonStyle())
            .fastHelp(controller.isPanelLocked ? L10n.unpinPanel : L10n.pinPanel)

            Button {
                controller.dismissPanel()
                panelLockVersion = UUID()
            } label: {
                Image(systemName: "xmark")
            }
            .buttonStyle(HeaderButtonStyle(hoverColor: .red))
            .fastHelp(L10n.closePanel, horizontalOffset: -32)
        }
        .padding(.horizontal, 16)
        .frame(height: 48)
        .zIndex(10)
    }

    private var listContent: some View {
        VStack(spacing: 0) {
            inputField
            if store.unfinished.isEmpty { emptyState }
            else { taskScroll }
        }
    }

    private var inputField: some View {
        HStack(spacing: 8) {
            TextField(L10n.placeholder, text: $input)
                .textFieldStyle(.plain)
                .font(.system(size: 14))
                .tint(Color.liteTickGreen)
                .focused($inputFocused)
                .onSubmit {
                    store.add(input)
                    input = ""
                    inputFocused = true
                }
            Button {
                store.add(input); input = ""; inputFocused = true
            } label: {
                Image(systemName: "plus").font(.system(size: 12, weight: .bold))
            }
            .buttonStyle(.plain)
            .foregroundStyle(input.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty ? Color.secondary : Color.liteTickGreen)
            .disabled(input.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty)
        }
        .padding(.horizontal, 13)
        .frame(height: 42)
        .background(Color.liteTickElevated, in: RoundedRectangle(cornerRadius: 3, style: .continuous))
        .overlay {
            RoundedRectangle(cornerRadius: 3, style: .continuous)
                .stroke(inputFocused ? Color.liteTickGreen : Color.liteTickSeparator, lineWidth: inputFocused ? 1.5 : 1)
        }
        .padding(.horizontal, 12)
        .padding(.vertical, 11)
    }

    private var emptyState: some View {
        VStack(spacing: 8) {
            Spacer()
            Image(systemName: "checklist").font(.system(size: 24, weight: .light)).foregroundStyle(Color.liteTickSecondaryText)
            Text(store.items.isEmpty ? L10n.emptyTitle : L10n.noPendingTitle)
                .font(.system(size: 13, weight: .medium))
            Text(store.items.isEmpty ? L10n.emptyHint : L10n.noPendingHint)
                .font(.system(size: 11))
                .foregroundStyle(Color.liteTickSecondaryText)
            Spacer()
        }
    }

    private var taskScroll: some View {
        ScrollView {
            LazyVStack(spacing: 0) {
                ForEach(store.unfinished) { item in
                    taskRow(item)
                }
                Color.clear
                    .frame(height: 18)
                    .overlay(alignment: .top) {
                        if dropAtEnd {
                            Rectangle().fill(Color.liteTickGreen).frame(height: 2).padding(.horizontal, 8)
                        }
                    }
            }
            .padding(.horizontal, 12)
            .padding(.bottom, 10)
        }
        .coordinateSpace(name: "taskList")
        .onPreferenceChange(TaskRowFramePreferenceKey.self) { taskRowFrames = $0 }
    }

    @ViewBuilder
    private func taskRow(_ item: TaskItem) -> some View {
        let row = TaskRow(
            item: item,
            onToggle: { store.toggle(item.id) },
            onEdit: { store.updateText(item.id, text: $0) },
            onTogglePinned: { store.togglePinned(item.id) },
            onDelete: { deleteCandidate = item },
            onReorderChanged: item.isPinned ? nil : { location in
                updateReorder(item.id, at: location)
            },
            onReorderEnded: item.isPinned ? nil : { _ in finishReorder() }
        )
        .id("unfinished-\(item.id.uuidString)-\(item.isPinned)")
        .background {
            GeometryReader { proxy in
                Color.clear.preference(
                    key: TaskRowFramePreferenceKey.self,
                    value: [item.id: proxy.frame(in: .named("taskList"))]
                )
            }
        }
        .overlay(alignment: .top) {
            if dropTargetID == item.id {
                Rectangle().fill(Color.liteTickGreen).frame(height: 2).padding(.horizontal, 8)
            }
        }

        row
    }

    private func updateReorder(_ itemID: UUID, at location: CGPoint) {
        draggedID = itemID
        let orderedFrames = taskRowFrames
            .filter { $0.key != itemID }
            .sorted { $0.value.minY < $1.value.minY }

        if let target = orderedFrames.first(where: { location.y < $0.value.midY }) {
            dropTargetID = target.key
            dropAtEnd = false
        } else {
            dropTargetID = nil
            dropAtEnd = true
        }
    }

    private func finishReorder() {
        defer {
            draggedID = nil
            dropTargetID = nil
            dropAtEnd = false
        }
        guard let draggedID else { return }
        if dropAtEnd { store.moveToEnd(draggedID) }
        else if let dropTargetID { store.move(draggedID, before: dropTargetID) }
    }

}

private struct TaskRow: View {
    let item: TaskItem
    let onToggle: () -> Void
    let onEdit: (String) -> Void
    let onTogglePinned: () -> Void
    let onDelete: () -> Void
    let onReorderChanged: ((CGPoint) -> Void)?
    let onReorderEnded: ((CGPoint) -> Void)?
    @State private var hovering = false
    @State private var showingCompletion = false
    @State private var editing = false
    @State private var draft: String

    private var checked: Bool { item.isCompleted || showingCompletion }

    init(item: TaskItem, onToggle: @escaping () -> Void, onEdit: @escaping (String) -> Void, onTogglePinned: @escaping () -> Void, onDelete: @escaping () -> Void, onReorderChanged: ((CGPoint) -> Void)?, onReorderEnded: ((CGPoint) -> Void)?) {
        self.item = item
        self.onToggle = onToggle
        self.onEdit = onEdit
        self.onTogglePinned = onTogglePinned
        self.onDelete = onDelete
        self.onReorderChanged = onReorderChanged
        self.onReorderEnded = onReorderEnded
        _draft = State(initialValue: item.text)
    }

    var body: some View {
        HStack(alignment: .top, spacing: 10) {
            Button {
                if item.isCompleted { onToggle() }
                else {
                    showingCompletion = true
                    DispatchQueue.main.asyncAfter(deadline: .now() + 0.38) { onToggle(); showingCompletion = false }
                }
            } label: {
                Image(systemName: checked ? "checkmark.square.fill" : "square")
                    .font(.system(size: 17)).foregroundStyle(checked ? Color.liteTickGreen : Color.secondary)
            }.buttonStyle(.plain)
            if editing {
                EndFocusedTextField(text: $draft, onCommit: commitEdit, onBlur: commitEdit)
                    .frame(maxWidth: .infinity, minHeight: 24, alignment: .leading)
            } else {
                Text(item.text)
                    .font(.system(size: 13))
                    .foregroundStyle(checked ? Color.secondary : Color.primary)
                    .strikethrough(checked, color: .secondary)
                    .fixedSize(horizontal: false, vertical: true)
                    .frame(maxWidth: .infinity, minHeight: 24, alignment: .leading)
                    .contentShape(Rectangle())
                    .onTapGesture { beginEdit() }
                    .onHover { NSCursor.setForTextRegion($0) }
            }
            if !editing {
                HStack(spacing: 4) {
                    Button(action: onTogglePinned) {
                        RowActionIcon(systemName: "arrow.up.to.line", weight: .semibold, hoverColor: .liteTickGreen)
                    }
                    .buttonStyle(.plain)
                    .foregroundStyle(Color.liteTickGreen.opacity(item.isPinned ? 1 : 0.88))
                    .opacity(item.isPinned || hovering ? 1 : 0)
                    .allowsHitTesting(item.isPinned || hovering)
                    .accessibilityHidden(!(item.isPinned || hovering))
                    .help(item.isPinned ? L10n.unpinItem : L10n.pinItem)
                }

                if let onReorderChanged, let onReorderEnded {
                    ReorderGrip()
                        .frame(width: 24, height: 24)
                        .contentShape(Rectangle())
                        .opacity(hovering ? 1 : 0)
                        .allowsHitTesting(hovering)
                        .accessibilityHidden(!hovering)
                        .help(L10n.dragToReorder)
                        .gesture(
                            DragGesture(minimumDistance: 3, coordinateSpace: .named("taskList"))
                                .onChanged {
                                    NSCursor.closedHand.set()
                                    onReorderChanged($0.location)
                                }
                                .onEnded {
                                    NSCursor.openHand.set()
                                    onReorderEnded($0.location)
                                }
                        )
                }
            }
        }
        .frame(minHeight: 24, alignment: .top)
        .padding(.horizontal, 10).padding(.vertical, 11)
        .background(rowBackground)
        .overlay(alignment: .bottom) {
            Rectangle()
                .fill(Color.liteTickSeparator.opacity(0.72))
                .frame(height: 0.5)
        }
        .contentShape(Rectangle()).onHover { hovering = $0 }
        .animation(.easeOut(duration: 0.18), value: showingCompletion)
        .contextMenu {
            Button(L10n.edit, action: beginEdit)
            Button(item.isPinned ? L10n.unpinItem : L10n.pinItem, action: onTogglePinned)
            Button(L10n.delete, role: .destructive, action: onDelete)
        }
    }

    private var rowBackground: Color {
        if item.isPinned { return Color.liteTickSoftGreen }
        return hovering ? Color.primary.opacity(0.045) : Color.clear
    }

    private func beginEdit() {
        draft = item.text
        editing = true
    }

    private func commitEdit() {
        guard editing else { return }
        let cleaned = draft.trimmingCharacters(in: .whitespacesAndNewlines)
        if !cleaned.isEmpty { onEdit(cleaned) } else { draft = item.text }
        editing = false
    }
}

private struct EndFocusedTextField: NSViewRepresentable {
    @Binding var text: String
    let onCommit: () -> Void
    let onBlur: () -> Void

    func makeCoordinator() -> Coordinator {
        Coordinator(text: $text, onCommit: onCommit, onBlur: onBlur)
    }

    func makeNSView(context: Context) -> NSTextField {
        let field = NSTextField()
        field.isBezeled = false
        field.drawsBackground = false
        field.focusRingType = .none
        field.font = .systemFont(ofSize: 13)
        field.lineBreakMode = .byTruncatingTail
        field.stringValue = text
        field.delegate = context.coordinator

        DispatchQueue.main.async { [weak field] in
            guard let field, let window = field.window else { return }
            window.makeFirstResponder(field)
            guard let editor = window.fieldEditor(true, for: field) as? NSTextView else { return }
            editor.insertionPointColor = .liteTickGreen
            editor.setSelectedRange(NSRange(location: editor.string.utf16.count, length: 0))
        }
        return field
    }

    func updateNSView(_ field: NSTextField, context: Context) {
        if field.stringValue != text { field.stringValue = text }
    }

    final class Coordinator: NSObject, NSTextFieldDelegate {
        @Binding var text: String
        let onCommit: () -> Void
        let onBlur: () -> Void

        init(text: Binding<String>, onCommit: @escaping () -> Void, onBlur: @escaping () -> Void) {
            _text = text
            self.onCommit = onCommit
            self.onBlur = onBlur
        }

        func controlTextDidChange(_ notification: Notification) {
            guard let field = notification.object as? NSTextField else { return }
            text = field.stringValue
        }

        func controlTextDidEndEditing(_ notification: Notification) {
            onBlur()
        }

        func control(_ control: NSControl, textView: NSTextView, doCommandBy commandSelector: Selector) -> Bool {
            guard commandSelector == #selector(NSResponder.insertNewline(_:)) else { return false }
            onCommit()
            return true
        }
    }
}

private struct HistoryView: View {
    @Bindable var store: TaskStore
    @Binding var deleteCandidate: TaskItem?
    @State private var search = ""
    @State private var showingDeleteAllConfirmation = false
    @State private var deletionTargetIDs: Set<UUID> = []
    @State private var deletingSearchResults = false

    private var results: [TaskItem] {
        let query = search.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !query.isEmpty else { return store.completed }
        return store.completed.filter { $0.text.localizedCaseInsensitiveContains(query) }
    }

    private var hasSearch: Bool {
        !search.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty
    }

    var body: some View {
        VStack(spacing: 0) {
            HStack {
                Image(systemName: "magnifyingglass").foregroundStyle(.secondary)
                TextField(L10n.search, text: $search)
                    .textFieldStyle(.plain)
                    .tint(Color.liteTickGreen)
                if !results.isEmpty {
                    Button(hasSearch ? L10n.deleteResults : L10n.deleteAll) {
                        deletionTargetIDs = Set(results.map(\.id))
                        deletingSearchResults = hasSearch
                        showingDeleteAllConfirmation = true
                    }
                        .buttonStyle(.plain)
                        .font(.system(size: 11))
                        .foregroundStyle(.red)
                }
            }
            .padding(.horizontal, 12).frame(height: 38)
            .background(Color.liteTickElevated, in: RoundedRectangle(cornerRadius: 3, style: .continuous))
            .overlay {
                RoundedRectangle(cornerRadius: 3, style: .continuous)
                    .stroke(Color.liteTickSeparator, lineWidth: 1)
            }
            .padding(12)

            if results.isEmpty {
                Spacer()
                Text(store.completed.isEmpty ? L10n.noCompleted : L10n.noMatches)
                    .foregroundStyle(.secondary)
                Spacer()
            } else {
                ScrollView {
                    LazyVStack(spacing: 0) {
                        ForEach(results) { item in
                            CompletedRow(
                                item: item,
                                dateText: item.completedAt.map(formattedDate),
                                onRestore: { store.toggle(item.id) },
                                onDelete: { deleteCandidate = item }
                            )
                        }
                    }.padding(.horizontal, 12).padding(.bottom, 12)
                }
            }
        }
        .alert(deletingSearchResults ? L10n.deleteResultsTitle : L10n.deleteAllTitle, isPresented: $showingDeleteAllConfirmation) {
            Button(L10n.cancel, role: .cancel) {}
            Button(deletingSearchResults ? L10n.deleteResults : L10n.deleteAll, role: .destructive) {
                store.deleteCompleted(deletionTargetIDs)
                deletionTargetIDs = []
                search = ""
            }
        } message: {
            Text(deletingSearchResults ? L10n.deleteResultsMessage : L10n.deleteAllMessage)
        }
    }

    private func formattedDate(_ date: Date) -> String {
        let locale = Locale(identifier: L10n.isChinese ? "zh_CN" : "en_US")
        return date.formatted(
            Date.FormatStyle(date: .abbreviated, time: .omitted)
                .locale(locale)
        )
    }
}

private struct CompletedRow: View {
    let item: TaskItem
    let dateText: String?
    let onRestore: () -> Void
    let onDelete: () -> Void
    @State private var hovering = false

    var body: some View {
        HStack(alignment: .top, spacing: 10) {
            Button(action: onRestore) {
                Image(systemName: "checkmark.square.fill")
                    .font(.system(size: 17))
                    .foregroundStyle(Color.liteTickGreen)
            }
            .buttonStyle(.plain)
            .help(L10n.restoreToList)

            VStack(alignment: .leading, spacing: 3) {
                Text(item.text)
                    .font(.system(size: 13))
                    .lineLimit(2)
                    .frame(maxWidth: .infinity, alignment: .leading)
                if let dateText {
                    Text(dateText)
                        .font(.system(size: 10))
                        .foregroundStyle(.secondary)
                }
            }

            Button(action: onDelete) {
                RowActionIcon(systemName: "trash", weight: .medium, hoverColor: .red)
            }
            .buttonStyle(.plain)
            .foregroundStyle(Color.red.opacity(0.78))
            .opacity(hovering ? 1 : 0)
            .allowsHitTesting(hovering)
            .accessibilityHidden(!hovering)
            .help(L10n.delete)
        }
        .frame(minHeight: 24, alignment: .top)
        .padding(.horizontal, 10)
        .padding(.vertical, 11)
        .background(hovering ? Color.primary.opacity(0.045) : Color.clear)
        .overlay(alignment: .bottom) {
            Rectangle()
                .fill(Color.liteTickSeparator.opacity(0.72))
                .frame(height: 0.5)
        }
        .contentShape(Rectangle())
        .onHover { hovering = $0 }
        .contextMenu {
            Button(L10n.restoreToList, action: onRestore)
            Button(L10n.delete, role: .destructive, action: onDelete)
        }
    }
}

private struct ReorderGrip: View {
    var body: some View {
        VStack(spacing: 2.5) {
            ForEach(0..<3, id: \.self) { _ in
                HStack(spacing: 2.5) {
                    Circle().frame(width: 2.5, height: 2.5)
                    Circle().frame(width: 2.5, height: 2.5)
                }
            }
        }
        .foregroundStyle(.tertiary)
        .frame(width: 24, height: 24)
        .onHover { hovering in
            if hovering { NSCursor.openHand.set() }
            else { NSCursor.arrow.set() }
        }
    }
}

private struct RowActionIcon: View {
    let systemName: String
    let weight: Font.Weight
    let hoverColor: Color
    @State private var hovering = false

    var body: some View {
        Image(systemName: systemName)
            .symbolRenderingMode(.monochrome)
            .font(.system(size: 13, weight: weight))
            .frame(width: 24, height: 24)
            .background(
                hoverColor.opacity(hovering ? 0.12 : 0),
                in: RoundedRectangle(cornerRadius: 5, style: .continuous)
            )
            .contentShape(Rectangle())
            .onHover { hovering = $0 }
    }
}

private extension NSCursor {
    static func setForTextRegion(_ hovering: Bool) {
        if hovering { NSCursor.iBeam.set() }
        else { NSCursor.arrow.set() }
    }
}

private struct HeaderButtonStyle: ButtonStyle {
    var hoverColor: Color = .primary

    func makeBody(configuration: Configuration) -> some View {
        HeaderButtonBody(configuration: configuration, hoverColor: hoverColor)
    }

    private struct HeaderButtonBody: View {
        let configuration: Configuration
        let hoverColor: Color
        @State private var hovering = false

        var body: some View {
            configuration.label
                .symbolRenderingMode(.monochrome)
                .font(.system(size: 13, weight: .medium))
                .frame(width: 28, height: 28)
                .background(
                    hoverColor.opacity(configuration.isPressed ? 0.12 : hovering ? 0.08 : 0),
                    in: RoundedRectangle(cornerRadius: 5, style: .continuous)
                )
                .contentShape(Rectangle())
                .onHover { hovering = $0 }
        }
    }
}

private struct FastHelpModifier: ViewModifier {
    let text: String
    let horizontalOffset: CGFloat

    @State private var isPresented = false
    @State private var hoverToken = UUID()

    func body(content: Content) -> some View {
        content
            .accessibilityLabel(text)
            .onHover { hovering in
                let token = UUID()
                hoverToken = token
                if hovering {
                    DispatchQueue.main.asyncAfter(deadline: .now() + 0.16) {
                        guard hoverToken == token else { return }
                        isPresented = true
                    }
                } else {
                    isPresented = false
                }
            }
            .overlay(alignment: .bottom) {
                if isPresented {
                    Text(text)
                        .font(.system(size: 10.5, weight: .regular))
                        .foregroundStyle(.primary)
                        .lineLimit(1)
                        .fixedSize()
                        .padding(.horizontal, 7)
                        .padding(.vertical, 4)
                        .background(.regularMaterial, in: RoundedRectangle(cornerRadius: 5, style: .continuous))
                        .overlay {
                            RoundedRectangle(cornerRadius: 5, style: .continuous)
                                .stroke(Color.liteTickSeparator, lineWidth: 0.5)
                        }
                        .shadow(color: .black.opacity(0.12), radius: 3, y: 1)
                        .offset(x: horizontalOffset, y: 28)
                        .allowsHitTesting(false)
                }
            }
            .zIndex(isPresented ? 20 : 0)
    }
}

private extension View {
    func fastHelp(_ text: String, horizontalOffset: CGFloat = 0) -> some View {
        modifier(FastHelpModifier(text: text, horizontalOffset: horizontalOffset))
    }
}

private struct TaskRowFramePreferenceKey: PreferenceKey {
    static let defaultValue: [UUID: CGRect] = [:]

    static func reduce(value: inout [UUID: CGRect], nextValue: () -> [UUID: CGRect]) {
        value.merge(nextValue(), uniquingKeysWith: { _, new in new })
    }
}
