import Foundation

enum ProductIdentity {
    static let chineseName = Bundle.main.object(forInfoDictionaryKey: "LiteTickChineseName") as? String ?? "妥了"
    static let englishName = Bundle.main.object(forInfoDictionaryKey: "LiteTickEnglishName") as? String ?? "LiteTick"
    static let dataDirectoryName = Bundle.main.object(forInfoDictionaryKey: "LiteTickDataDirectory") as? String ?? "LiteTick"
}
